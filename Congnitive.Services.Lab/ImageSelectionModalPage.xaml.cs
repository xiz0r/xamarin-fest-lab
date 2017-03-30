using System;
using Xamarin.Forms;

namespace Cognitive.Services.Lab
{
	public partial class ImageSelectionModalPage : ContentPage
	{
		public event EventHandler SelectedItem;

		public ImageSelectionModalPage()
		{
			InitializeComponent();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			ShowActivityIndicator(true);
			lstImages.ItemsSource = await AzureStorage.GetBlobImages();
			ShowActivityIndicator(false);
		}

		private async void CloseButton_Clicked(object sender, System.EventArgs e)
		{
			if (lstImages.SelectedItem != null)
				SelectedItem?.Invoke(lstImages.SelectedItem, null);
			await Navigation.PopModalAsync();
		}

		private void ShowActivityIndicator(bool show)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				Indicator1.IsVisible = show;
				Indicator1.IsRunning = show;
			});
		}
	}
}