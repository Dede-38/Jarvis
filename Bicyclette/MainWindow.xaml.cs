using Bicyclette.Models;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bicyclette
{
    public partial class MainWindow : Window
    {
        private enum EtatMicro
        {
            Inactif,
            ActifDesactive,
            ActifEtParle
        }

        private EtatMicro etatMicro = EtatMicro.Inactif;
        private Conversation conversationActive;
        private SpeechListener speechListener;

        public MainWindow()
        {
            InitializeComponent();

            // ======= INITIALISATION RECONNAISSANCE VOCALE =======
            speechListener = new SpeechListener(); // ⚠️ Important : instanciation

            speechListener.OnSpeechRecognized += text =>
            {
                Console.WriteLine("🗣️ Reconnu : " + text);

                Dispatcher.Invoke(() =>
                {
                    if (etatMicro == EtatMicro.ActifEtParle)
                    {
                        if (text.ToLower().Contains("jarvis"))
                        {
                            InputTextBox.Text = text;
                            Envoyer_Click(null, null);
                        }
                        else
                        {
                            InputTextBox.Text += " " + text;
                        }
                    }
                });
            };
        }


        public static bool IsMicrophoneAvailable()
        {
            try
            {
                var enumerator = new MMDeviceEnumerator();
                var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                return devices.Any();
            }
            catch
            {
                return false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConversationManager.Load();
            etatMicro = IsMicrophoneAvailable() ? EtatMicro.ActifDesactive : EtatMicro.Inactif;
            MettreAJourAffichageMicro();
            TraductionManager.Langue = Properties.Settings.Default.Langue;
            AppliquerLangue();

            ConversationListBox.ItemsSource = null;
            ConversationListBox.ItemsSource = ConversationManager.Conversations;
            ModelLabel.Text = $"🧠 {TraductionManager.T("ModèleIA")} : {AppSettings.SelectedModel}";

            if (ConversationManager.Conversations.Any())
            {
                conversationActive = ConversationManager.Conversations.First();
                AfficherConversation();
            }
            else
            {
                conversationActive = CreerNouvelleConversation();
            }

        }

        // ======= LANGUE (Traduction dynamique) =======
        internal void AppliquerLangue()
        {
            ModelLabel.Text = $"🧠 {TraductionManager.T("ModèleIA")} : {AppSettings.SelectedModel}";
            NouvelleConversationButton.Content = TraductionManager.T("Nouvelle");
            SupprimerConversationButton.Content = TraductionManager.T("Supprimer");

            // Tooltips des boutons
            MicroButton.ToolTip = TraductionManager.T("MicroBoutonTooltip");
            if (AjouterImageButton != null)
                AjouterImageButton.ToolTip = TraductionManager.T("ImageBoutonTooltip");
            SettingsButton.ToolTip = TraductionManager.T("ParametresBoutonTooltip");
        }


        // ======= MICRO - Affichage selon l'état =======
        private void MettreAJourAffichageMicro()
        {
            switch (etatMicro)
            {
                case EtatMicro.Inactif:
                    MicroButton.Content = "🎤 (Off)";
                    MicroButton.Background = Brushes.Gray;
                    break;
                case EtatMicro.ActifDesactive:
                    MicroButton.Content = "🎤";
                    MicroButton.Background = (Brush)Application.Current.Resources["WindowBackgroundBrush"];
                    break;
                case EtatMicro.ActifEtParle:
                    MicroButton.Content = "🔴🎤";
                    MicroButton.Background = Brushes.Red;
                    break;
            }
        }


        private void MicroButton_Click(object sender, RoutedEventArgs e)
        {
            if (etatMicro == EtatMicro.Inactif) return;

            if (!IsMicrophoneAvailable())
            {
                MessageBox.Show("Aucun micro n'est disponible sur ce système.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (etatMicro == EtatMicro.ActifDesactive)
            {
                speechListener.Start();
                etatMicro = EtatMicro.ActifEtParle;
            }
            else if (etatMicro == EtatMicro.ActifEtParle)
            {
                speechListener.Stop();
                etatMicro = EtatMicro.ActifDesactive;
            }

            MettreAJourAffichageMicro();
        }



        // ======= BOUTON NOUVELLE CONVERSATION =======
        private void NouvelleConversation_Click(object sender, RoutedEventArgs e)
        {
            conversationActive = new Conversation
            {
                Id = Guid.NewGuid().ToString(),
                Title = TraductionManager.T("Nouvelle")
            };

            ConversationManager.Conversations.Add(conversationActive);
            ConversationManager.Save();

            ConversationListBox.ItemsSource = null;
            ConversationListBox.ItemsSource = ConversationManager.Conversations;
            ConversationListBox.SelectedItem = conversationActive;

            ConversationTextBlock.Text = string.Empty;
        }

        // ======= BOUTON ENTRÉE CLAVIER (ENVOI) =======
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Envoyer_Click(sender, e);
            }
        }

        // ======= RENOMMER CONVERSATION =======
        private void RenommerConversation_Click(object sender, RoutedEventArgs e)
        {
            if (ConversationListBox.SelectedItem is Conversation conv)
            {
                string nouveauNom = Microsoft.VisualBasic.Interaction.InputBox(
                    TraductionManager.T("PromptNom"),
                    TraductionManager.T("TitreRenommage"),
                    conv.Title);

                if (!string.IsNullOrWhiteSpace(nouveauNom))
                {
                    conv.Title = nouveauNom.Trim();
                    ConversationManager.Save();

                    ConversationListBox.ItemsSource = null;
                    ConversationListBox.ItemsSource = ConversationManager.Conversations;
                    ConversationListBox.SelectedItem = conv;
                }
            }
        }

        // ======= ENVOYER MESSAGE =======
        private async void Envoyer_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            InputTextBox.Clear();
            InputTextBox.IsEnabled = false;

            // Historique pour OpenAI
            List<Message> historique = new();
            foreach (string msg in conversationActive.Messages)
            {
                if (msg.StartsWith("👤: "))
                    historique.Add(new Message { role = "user", content = msg.Substring(4) });
                else if (msg.StartsWith("🤖: "))
                    historique.Add(new Message { role = "assistant", content = msg.Substring(4) });
            }

            historique.Add(new Message { role = "user", content = input });

            conversationActive.Messages.Add("👤: " + input);
            ConversationTextBlock.Text = string.Join("\n", conversationActive.Messages);
            ConversationTextBlock.Text += $"\n🤖: {TraductionManager.T("Typing")}";

            try
            {
                string reponse = await EnvoyerPromptChatGPT(historique);
                conversationActive.Messages.Add("🤖: " + reponse);
                ConversationTextBlock.Text = string.Join("\n", conversationActive.Messages);
                ConversationManager.Save();
            }
            catch (Exception ex)
            {
                string erreur = TraductionManager.T("ErreurIA");
                conversationActive.Messages.Add("🤖: " + erreur);
                ConversationTextBlock.Text = string.Join("\n", conversationActive.Messages);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                InputTextBox.IsEnabled = true;
                InputTextBox.Focus();
            }
        }


        // ======= AFFICHER CONVERSATION =======
        private void AfficherConversation()
        {
            ConversationTextBlock.Text = string.Join("\n", conversationActive.Messages);
        }

        private async Task<string> EnvoyerPromptChatGPT(List<Message> history)
        {
            if (string.IsNullOrWhiteSpace(AppSettings.ApiKey))
            {
                MessageBox.Show("❌ Clé API manquante. Veuillez la définir dans les paramètres.", "Erreur API", MessageBoxButton.OK, MessageBoxImage.Warning);
                return "Erreur : clé API manquante.";
            }

            string url = "https://api.openai.com/v1/chat/completions";

            var requestBody = new
            {
                model = AppSettings.SelectedModel,
                messages = history
            };

            string jsonBody = JsonSerializer.Serialize(requestBody);

            using var httpClient = new HttpClient();

            try
            {
                httpClient.DefaultRequestHeaders.Clear(); // ← important pour éviter les erreurs
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppSettings.ApiKey);

                using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                using var response = await httpClient.PostAsync(url, content);

                string responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    string apiErrorMessage = "";
                    try
                    {
                        using JsonDocument doc = JsonDocument.Parse(responseJson);
                        if (doc.RootElement.TryGetProperty("error", out var error))
                        {
                            apiErrorMessage = error.GetProperty("message").GetString();
                        }
                    }
                    catch { }

                    MessageBox.Show(
                        $"Erreur lors de la connexion à l'IA\n\n🔁 Code HTTP : {(int)response.StatusCode} {response.StatusCode}\n📩 Détail : {apiErrorMessage}",
                        "Erreur API"

                    );

                    return $"Erreur IA : {apiErrorMessage}";
                }

                using JsonDocument docSuccess = JsonDocument.Parse(responseJson);
                string message = docSuccess.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return message?.Trim() ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception lors de l'envoi : " + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return "Erreur inattendue : " + ex.Message;
            }
        }




        // ======= CHANGEMENT CONVERSATION =======
        private void ConversationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConversationListBox.SelectedItem is Conversation conv)
            {
                conversationActive = conv;
                AfficherConversation();
            }
        }

        // ======= BOUTON SUPPRIMER CONVERSATION =======
        private void SupprimerConversation_Click(object sender, RoutedEventArgs e)
        {
            if (ConversationListBox.SelectedItem is Conversation conv)
            {
                if (MessageBox.Show(
                        TraductionManager.T("ConfirmationSuppression"),
                        TraductionManager.T("TitreConfirmation"),
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ConversationManager.Conversations.Remove(conv);
                    ConversationManager.Save();

                    ConversationListBox.ItemsSource = null;
                    ConversationListBox.ItemsSource = ConversationManager.Conversations;

                    conversationActive = ConversationManager.Conversations.FirstOrDefault()
                                        ?? CreerNouvelleConversation();
                    AfficherConversation();
                }
            }
        }

        // ======= CRÉER CONVERSATION VIDE =======
        private Conversation CreerNouvelleConversation()
        {
            var conv = new Conversation
            {
                Id = Guid.NewGuid().ToString(),
                Title = TraductionManager.T("Nouvelle")
            };

            ConversationManager.Conversations.Add(conv);
            ConversationManager.Save();
            ConversationListBox.ItemsSource = null;
            ConversationListBox.ItemsSource = ConversationManager.Conversations;

            return conv;
        }

        // ======= MODÈLE IA LABEL =======
        private void MettreAJourAffichageModelIA()
        {
            ModelLabel.Text = $"🧠 {TraductionManager.T("ModèleIA")} : {AppSettings.SelectedModel}";
        }

        // ======= BOUTON PARAMÈTRES =======
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var fenetreParam = new Parametres(this);  // <-- on passe 'this' (la MainWindow)
            var fenetre = new Window
            {
                Content = fenetreParam,
                Width = 300,
                Height = 300,
                Title = "Paramètres",
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            fenetre.ShowDialog();

            MettreAJourAffichageModelIA();
        }



        // ======= BOUTON AJOUTER IMAGE =======
        private void AjouterImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Images (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";

            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(filePath);
                image.DecodePixelWidth = 300;
                image.EndInit();

                Image imageControl = new Image
                {
                    Source = image,
                    MaxWidth = 300,
                    Margin = new Thickness(0, 10, 0, 10)
                };

                ConversationTextBlock.Inlines.Add(new LineBreak());
                ConversationTextBlock.Inlines.Add(new Run(TraductionManager.T("ImageAjoutee")));
                ConversationTextBlock.Inlines.Add(new LineBreak());
                ConversationTextBlock.Inlines.Add(new InlineUIContainer(imageControl));
                ConversationTextBlock.Inlines.Add(new LineBreak());

                conversationActive.Messages.Add(TraductionManager.T("ImageInseree"));
                ConversationManager.Save();
            }
        }
    }
}
