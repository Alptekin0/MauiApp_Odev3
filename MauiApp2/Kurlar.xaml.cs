using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiApp2
{
    public partial class Kurlar : ContentPage
    {
        private ObservableCollection<Doviz> AllDovizList { get; set; } = new ObservableCollection<Doviz>();

        public ObservableCollection<Doviz> FilteredDovizList { get; set; } = new ObservableCollection<Doviz>();

        public Kurlar()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Load();
        }

        AltinDoviz kurlar;

        async Task Load()
        {
            try
            {
                string jsondata = await GetAltinDovizGuncelKurlar();
                kurlar = JsonSerializer.Deserialize<AltinDoviz>(jsondata);

                AllDovizList.Clear();
                FilteredDovizList.Clear();

                foreach (var currency in kurlar.Currencies)
                {

                    if (currency.Value.TryGetProperty("Alýþ", out JsonElement alisElement) &&
                        currency.Value.TryGetProperty("Satýþ", out JsonElement satisElement) &&
                        currency.Value.TryGetProperty("Deðiþim", out JsonElement degisimElement))
                    {
                        string dovizAdi = GetCurrencyName(currency.Key);
                        string alis = alisElement.GetString();
                        string satis = satisElement.GetString();
                        string degisim = degisimElement.GetString();

                        var doviz = new Doviz()
                        {
                            doviz_adi = dovizAdi,
                            doviz_alis = alis,
                            doviz_satis = satis,
                            Fark = degisim,
                            Yon = GetImage(degisim)
                        };

                        AllDovizList.Add(doviz);
                        FilteredDovizList.Add(doviz);
                    }
                }

                Sepet.ItemsSource = FilteredDovizList;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Veri yüklenirken bir hata oluþtu: {ex.Message}", "Tamam");
            }
        }

        private string GetCurrencyName(string code)
        {
            
            switch (code.ToUpper())
            {
                case "USD":
                    return "Dolar";
                case "EUR":
                    return "Euro";
                case "GBP":
                    return "Sterlin";
                case "CHF":
                    return "Frang";
                case "CAD":
                    return "Kanada Dolarý";
                
                default:
                    return code;
            }
        }

        private string GetImage(string degisim)
        {
            if (degisim.Contains("-"))
                return "down.png";
            if (degisim.Contains("+"))
                return "up.png";
            return "minus.png";
        }

        async Task<string> GetAltinDovizGuncelKurlar()
        {
            string url = "https://finans.truncgil.com/today.json";
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"API isteði baþarýsýz oldu: {response.StatusCode}");
                }

                string jsondata = await response.Content.ReadAsStringAsync();
                return jsondata;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Load();
            await Task.Delay(1000);
        }


        private void CurrencySearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterDovizList(e.NewTextValue);
        }


        private void CurrencySearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            FilterDovizList(CurrencySearchBar.Text);
        }

        private void FilterDovizList(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {

                FilteredDovizList.Clear();
                foreach (var doviz in AllDovizList)
                {
                    FilteredDovizList.Add(doviz);
                }
            }
            else
            {

                var lowerFilter = filter.ToLower();
                var filtered = AllDovizList.Where(d => d.doviz_adi.ToLower().Contains(lowerFilter)).ToList();

                FilteredDovizList.Clear();
                foreach (var doviz in filtered)
                {
                    FilteredDovizList.Add(doviz);
                }
            }
        }
    }

    public class Doviz
    {
        public string doviz_adi { get; set; }
        public string doviz_alis { get; set; }
        public string doviz_satis { get; set; }
        public string Fark { get; set; }
        public string Yon { get; set; }


        public string Icon
        {
            get
            {
                switch (doviz_adi.ToLower())
                {
                    case "dolar":
                        return "usd.png";
                    case "euro":
                        return "eur.png";
                    case "sterlin":
                        return "gbp.png";
                    case "frang":
                        return "chf.png";
                    case "kanada dolarý":
                        return "cad.png";

                    default:
                        return "default.png";
                }
            }
        }

        public Color FarkColor
        {
            get
            {
                if (Fark.Contains("-"))
                    return Colors.Red;
                if (Fark.Contains("+"))
                    return Colors.Green;
                return Colors.Gray;
            }
        }
    }

    public class AltinDoviz
    {
        [JsonPropertyName("Update_Date")]
        public string UpdateDate { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Currencies { get; set; }
    }
}