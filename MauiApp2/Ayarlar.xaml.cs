using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Graphics;

namespace MauiApp2
{
    public partial class Ayarlar : ContentPage
    {
        public Ayarlar()
        {
            InitializeComponent();
            var isDarkMode = Preferences.Get("isDarkMode", false);
            ThemeSwitch.IsToggled = isDarkMode;
        }

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            bool isDarkMode = e.Value;
            App.SetAppTheme(isDarkMode);

            if (Application.Current.Resources.TryGetValue(isDarkMode ? "PrimaryIconColorDark" : "PrimaryIconColorLight", out var color))
            {
                Application.Current.Resources["PrimaryIconColor"] = color;
            }
            else
            {

                Application.Current.Resources["PrimaryIconColor"] = isDarkMode ? Colors.White : Colors.Black;
            }
        }
    }
}
