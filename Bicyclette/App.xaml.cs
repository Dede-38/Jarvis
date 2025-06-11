using System;
using System.Linq;
using System.Windows;

namespace Bicyclette
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string savedTheme = Bicyclette.Properties.Settings.Default.Theme;

            if (string.IsNullOrWhiteSpace(savedTheme))
                savedTheme = "Themes/LightTheme.xaml";

            var newDict = new ResourceDictionary
            {
                Source = new Uri(savedTheme, UriKind.Relative)
            };

            var existingDict = Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null &&
                                     (d.Source.OriginalString.Contains("LightTheme") ||
                                      d.Source.OriginalString.Contains("DarkTheme")));

            if (existingDict != null)
                Current.Resources.MergedDictionaries.Remove(existingDict);

            Current.Resources.MergedDictionaries.Add(newDict);
        }
    }
}
