using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiApp2
{
    public partial class Haberler : ContentPage
    {
        private Root _root;
        private Kategori _selectedKategori;

        public Haberler()
        {
            InitializeComponent();

            category.ItemsSource = Kategori.Liste;

            if (Kategori.Liste.Count > 0)
            {
                _selectedKategori = Kategori.Liste[0];
                category.SelectedItem = _selectedKategori;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_selectedKategori != null)
                await Load();
        }

        private async Task Load()
        {
            try
            {
                string jsonData = await HaberleriGetir(_selectedKategori);
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    await DisplayAlert("Hata", "Haberler yüklenemedi. Sunucu veri göndermedi.", "Tamam");
                    return;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                _root = JsonSerializer.Deserialize<Root>(jsonData, options);

                if (_root == null || _root.Items == null || _root.Status?.ToLower() != "ok")
                {
                    await DisplayAlert("Hata",
                        "Haberler yüklenirken bir hata oluþtu. (Root veya Items null ya da Status != 'ok')",
                        "Tamam");
                    return;
                }

                lsHaberler.ItemsSource = _root.Items;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Haberler yüklenirken bir hata oluþtu:\n{ex.Message}", "Tamam");
            }
        }

        private async void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.CurrentSelection.FirstOrDefault() as Kategori;
            if (selected == null) return;

            _selectedKategori = selected;
            await Load();
        }

        private async void LsHaberler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.CurrentSelection.FirstOrDefault() as Item;
            if (selectedItem == null) return;

            lsHaberler.SelectedItem = null;

            HaberOkumaSayfasi newsDetail = new HaberOkumaSayfasi(selectedItem);
            await Navigation.PushModalAsync(new NavigationPage(newsDetail));
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Load();
        }

        public static async Task<string> HaberleriGetir(Kategori ctg)
        {
            try
            {
                using HttpClient client = new HttpClient();
                string url = ctg == null
                    ? "https://api.rss2json.com/v1/api.json?rss_url=https://www.trthaber.com/ekonomi_articles.rss"
                    : $"https://api.rss2json.com/v1/api.json?rss_url={Uri.EscapeDataString(ctg.Link)}";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonData = await response.Content.ReadAsStringAsync();
                return jsonData;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hata HaberleriGetir: {ex.Message}");
                return null;
            }
        }
    }

    public class Kategori
    {
        public string Baslik { get; set; }
        public string Link { get; set; }

        public static List<Kategori> Liste { get; } = new List<Kategori>
        {
            new Kategori() { Baslik = "Ekonomi",         Link = "https://www.trthaber.com/ekonomi_articles.rss" },
            new Kategori() { Baslik = "Bilim Teknoloji", Link = "https://www.trthaber.com/bilim_teknoloji_articles.rss" },
            new Kategori() { Baslik = "Son Dakika",      Link = "https://www.trthaber.com/sondakika_articles.rss" },
            new Kategori() { Baslik = "Gündem",          Link = "https://www.trthaber.com/gundem_articles.rss" },
            new Kategori() { Baslik = "Eðitim",          Link = "https://www.trthaber.com/egitim_articles.rss" },
            new Kategori() { Baslik = "Spor",            Link = "https://www.trthaber.com/spor_articles.rss" }
        };
    }

    public class Enclosure
    {
        public string Link { get; set; }
        public string Type { get; set; }
    }

    public class Feed
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class Item
    {
        public string Title { get; set; }
        public string PubDate { get; set; }
        public string Link { get; set; }
        public string Guid { get; set; }
        public string Author { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public Enclosure Enclosure { get; set; }
        public List<string> Categories { get; set; }
        public string PlainDescription { get; set; }
        public string DescriptionAsHtml { get; set; }
    }

    public class Root
    {
        public string Status { get; set; }
        public Feed Feed { get; set; }
        public List<Item> Items { get; set; }
    }
}
