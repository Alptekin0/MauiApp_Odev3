using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace MauiApp2
{
    public partial class Yapilacaklar : ContentPage
    {
        public ObservableCollection<Gorev> Gorevler { get; set; }

        public Yapilacaklar()
        {
            InitializeComponent();
            Gorevler = new ObservableCollection<Gorev>();
            BindingContext = this;
        }

        private void EkleButonu_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(GorevGirdisi.Text))
            {
                Gorevler.Add(new Gorev
                {
                    Baslik = GorevGirdisi.Text,
                    Tarih = GorevTarihi.Date,
                    Tamamlandi = false
                });
                GorevGirdisi.Text = string.Empty;
            }
            else
            {
                DisplayAlert("Hata", "Görev metni boş olamaz!", "Tamam");
            }
        }

        private async void SilButonu_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var gorev = button?.CommandParameter as Gorev;
            if (gorev != null)
            {
                bool confirm = await DisplayAlert("Onayla", $"{gorev.Baslik} görevini silmek istediğinize emin misiniz?", "Evet", "Hayır");
                if (confirm)
                {
                    Gorevler.Remove(gorev);
                }
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var gorev = checkbox.BindingContext as Gorev;

            if (gorev != null)
            {
                gorev.Tamamlandi = e.Value;
            }
        }

        private void GorevGirdisi_Completed(object sender, EventArgs e)
        {
            EkleButonu_Clicked(sender, e);
        }
    }

    public class Gorev
    {
        public string Baslik { get; set; }
        public DateTime Tarih { get; set; }
        public bool Tamamlandi { get; set; }
    }
}
