using System;

namespace Bicyclette
{
    public static class AppSettings
    {
        public static string SelectedModel
        {
            get => Properties.Settings.Default.ModelGPT;
            set
            {
                Properties.Settings.Default.ModelGPT = value;
                Properties.Settings.Default.Save();
            }
        }


        public static string Theme
        {
            get => Properties.Settings.Default.Theme ?? "Themes/LightTheme.xaml";
            set => Properties.Settings.Default.Theme = value;
        }

        public static int TypingDelayMs
        {
            get => Properties.Settings.Default.TypingDelayMs > 0
                ? Properties.Settings.Default.TypingDelayMs
                : 700;
            set => Properties.Settings.Default.TypingDelayMs = value;
        }

        public static int ChatFontSize
        {
            get => Properties.Settings.Default.ChatFontSize > 0
                ? Properties.Settings.Default.ChatFontSize
                : 14;
            set => Properties.Settings.Default.ChatFontSize = value;
        }

        public static string ApiKey
        {
            get => Properties.Settings.Default.ApiKey;
            set
            {
                Properties.Settings.Default.ApiKey = value;
                Properties.Settings.Default.Save();
            }
        }

        public static void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}

