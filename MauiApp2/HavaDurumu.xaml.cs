using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiApp2
{
    public partial class HavaDurumu : ContentPage
    {
        private static readonly string dosyaismi = Path.Combine(FileSystem.Current.AppDataDirectory, "hdata.json");
        public ObservableCollection<SehirHavaDurumu> Sehirler { get; set; } = new ObservableCollection<SehirHavaDurumu>();

        public HavaDurumu()
        {
            InitializeComponent();
            BindingContext = this;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                if (File.Exists(dosyaismi))
                {
                    string data = await File.ReadAllTextAsync(dosyaismi);
                    var sehirlerFromFile = JsonSerializer.Deserialize<ObservableCollection<SehirHavaDurumu>>(data);
                    if (sehirlerFromFile != null)
                    {
                        Sehirler.Clear();
                        foreach (var sehir in sehirlerFromFile)
                        {
                            Sehirler.Add(sehir);
                        }
                        await DisplayAlert("Bilgi", $"{Sehirler.Count} þehir yüklendi.", "Tamam");
                    }
                    else
                    {
                        await DisplayAlert("Bilgi", "Veri dosyasýnda þehir bulunamadý.", "Tamam");
                    }
                }
                else
                {
                    await DisplayAlert("Bilgi", "Veri dosyasý bulunamadý. Örnek þehir ekleniyor.", "Tamam");
                    Sehirler.Add(new SehirHavaDurumu { Name = "BARTIN" });
                    await SaveDataAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Veri yüklenirken hata oluþtu: {ex.Message}", "Tamam")   ;
            }
        }

        private async Task EkleSehirAsync(string sehirAdi)
        {
            if (!string.IsNullOrWhiteSpace(sehirAdi))
            {
                sehirAdi = NormalizeSehirAdi(sehirAdi);
                if (!Sehirler.Any(s => s.Name.Equals(sehirAdi, StringComparison.OrdinalIgnoreCase)))
                {
                    Sehirler.Add(new SehirHavaDurumu { Name = sehirAdi });
                    await SaveDataAsync();
                    await DisplayAlert("Baþarýlý", $"{sehirAdi} þehri eklendi.", "Tamam");
                }
                else
                {
                    await DisplayAlert("Uyarý", "Þehir zaten eklenmiþ.", "Tamam");
                }
            }
        }

        private async void AddSehir_Completed(object sender, EventArgs e)
        {
            string sehir = SehirEntry.Text;
            if (!string.IsNullOrWhiteSpace(sehir))
            {
                await EkleSehirAsync(sehir);
                SehirEntry.Text = string.Empty;



            }
        }

        private async void add_sehir(Object sender, EventArgs e)
        {
            string sehir = await DisplayPromptAsync("Þehir Ekle", "Eklemek istediðiniz þehrin adýný giriniz:", "Ekle", "Ýptal");
            if (!string.IsNullOrWhiteSpace(sehir))
            {
                await EkleSehirAsync(sehir);
            }
        }

        private async void Sil(object sender, EventArgs e)
        {
            var silButton = sender as ImageButton;
            if (silButton != null)
            {
                var sehirName = silButton.CommandParameter?.ToString();
                if (!string.IsNullOrEmpty(sehirName))
                {
                    bool confirm = await DisplayAlert("Onayla", $"{sehirName} þehrini silmek istediðinize emin misiniz?", "Evet", "Hayýr");
                    if (confirm)
                    {
                        var sehirToRemove = Sehirler.FirstOrDefault(o => o.Name.Equals(sehirName, StringComparison.OrdinalIgnoreCase));
                        if (sehirToRemove != null)
                        {
                            Sehirler.Remove(sehirToRemove);
                            await SaveDataAsync();
                        }
                    }
                }
            }
        }

        private async void refresh_data(object sender, EventArgs e)
        {
            LoadData();
        }

        private async Task SaveDataAsync()
        {
            try
            {
                string data = JsonSerializer.Serialize(Sehirler);
                await File.WriteAllTextAsync(dosyaismi, data);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Veri kaydedilirken hata oluþtu: {ex.Message}", "Tamam");
            }
        }

        private string NormalizeSehirAdi(string sehir)
        {
            return sehir.ToUpper(System.Globalization.CultureInfo.CurrentCulture)
                        .Replace('Ç', 'C')
                        .Replace('Ð', 'G')
                        .Replace('Ý', 'I')
                        .Replace('Ö', 'O')
                        .Replace('Ü', 'U')
                        .Replace('Þ', 'S');

        }
    }

    public class SehirHavaDurumu
    { 
        public string Name { get; set; }
        public string Source => $"https://www.mgm.gov.tr/sunum/tahmin-klasik-5070.aspx?m={Name}&basla=1&bitir=5&rC=111&rZ=fff";  
    }
}
