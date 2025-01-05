using Microsoft.Maui.Storage;
using System;
using Microsoft.Maui.Controls;

namespace MauiApp2
{
    public partial class GirisYap : ContentPage
    {
        private readonly FirebaseAuthService _authService;

        public GirisYap()
        {
            InitializeComponent();
            _authService = new FirebaseAuthService();
        }

        private async void BtnGirisYap_Clicked(object sender, EventArgs e)
        {
            string email = entryEmail.Text?.Trim();
            string password = entryPassword.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Hata", "L�tfen email ve �ifre giriniz.", "Tamam");
                return;
            }

            try
            {
                var result = await _authService.SignInUserAsync(email, password);

                if (result.Success)
                {
                    Preferences.Set("isLoggedIn", true);
                    Preferences.Set("UserUid", result.Message);

                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await DisplayAlert("Giri� Hatas�","Yanl�� E-posta veya �ifre","Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Beklenmeyen bir hata olu�tu: {ex.Message}", "Tamam");
            }
        }

        private async void BtnKaydol_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Kaydol());
        }
    }
}