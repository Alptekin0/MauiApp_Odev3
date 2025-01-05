using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using System.Text.RegularExpressions;

namespace MauiApp2
{
    public partial class HaberOkumaSayfasi : ContentPage
    {
        private Item _item;

        public HaberOkumaSayfasi(Item item)
        {
            InitializeComponent();

            _item = item;

            _item.PlainDescription = StripHtml(_item.Content);

            _item.DescriptionAsHtml = $"<html><body>{_item.Content}</body></html>"; 

            BindingContext = _item;
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_item?.Link))
            {
                await Browser.OpenAsync(_item.Link, BrowserLaunchMode.SystemPreferred);
            }
            else
            {
                await DisplayAlert("Hata", "Habere ait baðlantý bulunamadý.", "Tamam");
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await Navigation.PopModalAsync();
            }
        }

        private string StripHtml(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
}
