    using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;

namespace MauiApp2
{
    public partial class Kaydol : ContentPage
    {
        private readonly FirebaseAuthService _authService;

        public Kaydol()
        {
            InitializeComponent();
            _authService = new FirebaseAuthService();
        }

        private async void BtnKaydol_Clicked(object sender, EventArgs e)
        {
            string email = entryEmail.Text?.Trim();
            string password = entryPassword.Text?.Trim();
            string confirmPassword = entryConfirmPassword.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                await DisplayAlert("Hata", "L�tfen t�m alanlar� doldurunuz.", "Tamam");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Hata", "�ifreler uyu�muyor.", "Tamam");
                return;
            }

            try
            {
                var result = await _authService.RegisterUserAsync(email, password);

                if (result.Success)
                {
                    await DisplayAlert("Ba�ar�l�", "Kay�t ba�ar�l�. Giri� yapabilirsiniz.", "Tamam");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Kay�t Hatas�", result.Message, "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Beklenmeyen bir hata olu�tu: {ex.Message}", "Tamam");
            }
        }
    }
}