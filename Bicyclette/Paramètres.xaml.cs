using System;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.Generic;


namespace Bicyclette
{
    public partial class Parametres : UserControl
    {
        private MainWindow mainWindow;  // Référence vers la fenêtre principale
        public Parametres(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            ApiKeyBox.Password = Properties.Settings.Default.ApiKey ?? "";
            foreach (ComboBoxItem item in LangueComboBox.Items)
            {
                if ((string)item.Tag == Properties.Settings.Default.Langue)
                {
                    LangueComboBox.SelectedItem = item;
                    break;
                }
            }
            string apiKey = Properties.Settings.Default.ApiKey;
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                ApiKeyBox.Password = apiKey;
                ChargerModelesDepuisAPI(apiKey);
            }

        }
        private async void ChargerModelesDepuisAPI(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show("Veuillez entrer une clé API valide.", "Clé manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                HttpResponseMessage response = await client.GetAsync("https://api.openai.com/v1/models");
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);

                ModelComboBox.Items.Clear();

                foreach (var model in jsonDoc.RootElement.GetProperty("data").EnumerateArray())
                {
                    string id = model.GetProperty("id").GetString();

                    // Filtrer uniquement les modèles GPT utiles
                    if (id.StartsWith("gpt"))
                    {
                        ModelComboBox.Items.Add(new ComboBoxItem { Content = id });
                    }
                }

                // Réappliquer la sélection précédente si elle existe
                var selected = AppSettings.SelectedModel;
                foreach (ComboBoxItem item in ModelComboBox.Items)
                {
                    if (item.Content.ToString() == selected)
                    {
                        ModelComboBox.SelectedItem = item;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des modèles depuis OpenAI :\n" + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            AppliquerTheme("Themes/DarkTheme.xaml");
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            AppliquerTheme("Themes/LightTheme.xaml");
        }

        private void AppliquerTheme(string cheminTheme)
        {
            var newDict = new ResourceDictionary { Source = new Uri(cheminTheme, UriKind.Relative) };
            var existingDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null &&
                                     (d.Source.OriginalString.Contains("LightTheme") ||
                                      d.Source.OriginalString.Contains("DarkTheme")));

            if (existingDict != null)
                Application.Current.Resources.MergedDictionaries.Remove(existingDict);

            Application.Current.Resources.MergedDictionaries.Add(newDict);

            Properties.Settings.Default.Theme = cheminTheme;
            Properties.Settings.Default.Save();
        }



        private void ModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelComboBox.SelectedItem is ComboBoxItem item)
            {
                string selectedModel = item.Content.ToString();
                AppSettings.SelectedModel = selectedModel; // Sauvegarde dans les paramètres globaux
                UpdateApercu();
            }
        }


        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            string langueChoisie = LangueComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(langueChoisie) && langueChoisie != Properties.Settings.Default.Langue)
            {
                Properties.Settings.Default.Langue = langueChoisie;
                Properties.Settings.Default.Save();
                TraductionManager.Langue = langueChoisie;
                ((MainWindow)Window.GetWindow(this))?.AppliquerLangue();
            }
            string newApiKey = ApiKeyBox.Password;
            if (!string.IsNullOrWhiteSpace(newApiKey) && newApiKey != Properties.Settings.Default.ApiKey)
            {
                Properties.Settings.Default.ApiKey = newApiKey;
                Properties.Settings.Default.Save();
            }


            Window.GetWindow(this)?.Close();
        }

        private void ApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ApiKeyBox.Password))
            {
                ChargerModelesDepuisAPI(ApiKeyBox.Password);
            }
        }


        private void LangueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (LangueComboBox.SelectedItem is ComboBoxItem item && item.Tag is string langue)
                {
                    Properties.Settings.Default.Langue = langue;
                    Properties.Settings.Default.Save();
                    TraductionManager.Langue = langue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'enregistrement de la langue : " + ex.Message);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void UpdateTypingDelayLabel()
        {
            TypingDelayLabel.Text = $"{(int)TypingDelaySlider.Value} ms";
        }

        private void UpdateFontSizeLabel()
        {
            FontSizeLabel.Text = $"{(int)FontSizeSlider.Value} pt";
        }

        private void UpdateApercu()
        {
            if (ModelComboBox.SelectedItem is ComboBoxItem item)
            {
                string model = item.Content.ToString();
                string apercu = model switch
                {
                    "gpt-3.5-turbo" => "Rapide et économique, adapté aux tâches simples.",
                    "gpt-4" => "Plus intelligent, plus précis",
                    "gpt-4o" => "Multimodal et ultra rapide.",
                    _ => "Modèle inconnu."
                };

                LabelApercu.Text = $"🧠 {model} : {apercu}";
            }
        }

        private ComboBoxItem GetComboBoxItemByContent(ComboBox combo, string content)
        {
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content.ToString().Equals(content, StringComparison.OrdinalIgnoreCase))
                    return item;
            }
            return null;
        }
    }
}
