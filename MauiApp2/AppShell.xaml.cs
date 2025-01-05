using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MauiApp2
{
    public partial class AppShell : Shell
    {
        private readonly FirebaseAuthService _authService;

        public AppShell()
        {
            InitializeComponent();
            _authService = new FirebaseAuthService();
        }

        private async void OnFlyoutItemTapped(object sender, EventArgs e)
        {
            if (sender is FlyoutItem flyoutItem)
            {
                string pageName = flyoutItem.Route;

                await Shell.Current.GoToAsync($"//{pageName}");
                Shell.Current.FlyoutIsPresented = false;
            }
        }

        private async void OnLogOutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Çıkış Yap", "Çıkış yapmak istediğinize emin misiniz?", "Evet", "Hayır");
            if (confirm)
            {
                Preferences.Set("isLoggedIn", false);
                Preferences.Remove("UserUid");

                //await _authService.SignOutAsync();

                Application.Current.MainPage = new NavigationPage(new GirisYap());
            }
        }
    }
}
