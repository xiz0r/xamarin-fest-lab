using System;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Cognitive.Services.Lab
{
	public partial class IdentifyPage : ContentPage
	{
		private MediaFile file;
		private ImageModel azureImage;

		public IdentifyPage()
		{
			InitializeComponent();
		}

		private async void TakePictureButton_Clicked(object sender, EventArgs e)
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await DisplayAlert("No Camera", "No camera available.", "OK");
				return;
			}

			file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				SaveToAlbum = true,
				Name = "test.jpg"
			});

			if (file == null)
				return;

			// Upload image to azure blob storage
			azureImage = await AzureStorage.UploadImage(file);
		}

		private async void UploadPictureButton_Clicked(object sender, EventArgs e)
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				await DisplayAlert("No upload", "Picking a photo is not supported.", "OK");
				return;
			}

			// AZURE IMAGES
			var modal = new ImageSelectionModalPage();
			await Navigation.PushModalAsync(modal);

			modal.SelectedItem += (s, se) =>
			{
				azureImage = (ImageModel)s;
				Image1.Source = ImageSource.FromUri(new Uri(azureImage.StorageUri));
			};
		}

		private async void IdentifyButton_Clicked(object sender, EventArgs e)
		{
			if (azureImage == null) return;

			ShowActivityIndicator(true);
			lstCandidates.ItemsSource = await CognitiveService.Instance.IdentifyFaceAsync(azureImage.StorageUri);
			ShowActivityIndicator(false);
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