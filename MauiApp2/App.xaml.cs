using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Graphics;

namespace MauiApp2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var isDarkMode = Preferences.Get("isDarkMode", false);
            var isLoggedIn = Preferences.Get("isLoggedIn", false);

            this.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;

            if (isLoggedIn)
            {
                
                MainPage = new AppShell();
            }
            else
            {
                
                MainPage = new NavigationPage(new GirisYap());
            }
        }

        public static void SetAppTheme(bool isDarkMode)
        {
            Preferences.Set("isDarkMode", isDarkMode);

            if (Current != null)
            {
                Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
            }
        }
    }
}
