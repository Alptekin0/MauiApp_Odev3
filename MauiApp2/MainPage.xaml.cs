namespace MauiApp2
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void menu_clicked(object sender, EventArgs e)
        {

            Shell.Current.FlyoutIsPresented = true;


            string pageName = "MainPage";


            switch (pageName)
            {
                case "Kurlar":
                    await Shell.Current.GoToAsync("//Kurlar");
                    break;
                case "Haberler":
                    await Shell.Current.GoToAsync("//Haberler");
                    break;
                case "HavaDurumu":
                    await Shell.Current.GoToAsync("//HavaDurumu");
                    break;
                case "Yapilacaklar":
                    await Shell.Current.GoToAsync("//Yapilacaklar");
                    break;
                case "Ayarlar":
                    await Shell.Current.GoToAsync("//Ayarlar");
                    break;
                default:
                    break;
            }


            Shell.Current.FlyoutIsPresented = false;
        }
    }

}