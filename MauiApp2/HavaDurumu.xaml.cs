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
                        await DisplayAlert("Bilgi", $"{Sehirler.Count} �ehir y�klendi.", "Tamam");
                    }
                    else
                    {
                        await DisplayAlert("Bilgi", "Veri dosyas�nda �ehir bulunamad�.", "Tamam");
                    }
                }
                else
                {
                    await DisplayAlert("Bilgi", "Veri dosyas� bulunamad�. �rnek �ehir ekleniyor.", "Tamam");
                    Sehirler.Add(new SehirHavaDurumu { Name = "BARTIN" });
                    await SaveDataAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Veri y�klenirken hata olu�tu: {ex.Message}", "Tamam")   ;
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
                    await DisplayAlert("Ba�ar�l�", $"{sehirAdi} �ehri eklendi.", "Tamam");
                }
                else
                {
                    await DisplayAlert("Uyar�", "�ehir zaten eklenmi�.", "Tamam");
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
            string sehir = await DisplayPromptAsync("�ehir Ekle", "Eklemek istedi�iniz �ehrin ad�n� giriniz:", "Ekle", "�ptal");
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
                    bool confirm = await DisplayAlert("Onayla", $"{sehirName} �ehrini silmek istedi�inize emin misiniz?", "Evet", "Hay�r");
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
                await DisplayAlert("Hata", $"Veri kaydedilirken hata olu�tu: {ex.Message}", "Tamam");
            }
        }

        private string NormalizeSehirAdi(string sehir)
        {
            return sehir.ToUpper(System.Globalization.CultureInfo.CurrentCulture)
                        .Replace('�', 'C')
                        .Replace('�', 'G')
                        .Replace('�', 'I')
                        .Replace('�', 'O')
                        .Replace('�', 'U')
                        .Replace('�', 'S');

        }
    }

    public class SehirHavaDurumu
    { 
        public string Name { get; set; }
        public string Source => $"https://www.mgm.gov.tr/sunum/tahmin-klasik-5070.aspx?m={Name}&basla=1&bitir=5&rC=111&rZ=fff";  
    }
}
