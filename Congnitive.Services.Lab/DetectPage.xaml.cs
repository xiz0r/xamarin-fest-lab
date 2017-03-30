using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Cognitive.Services.Lab
{
	public partial class DetectPage : ContentPage
	{
		private MediaFile file;
		private ImageModel azureImage;

		public DetectPage()
		{
			InitializeComponent();
		}

		private async Task<FaceEmotionDetection> DetectFaceAndEmotionsAsync(string inputFile)
		{
			if (!CrossConnectivity.Current.IsConnected)
			{
				await DisplayAlert("Network error", "Please check your network connection and retry.", "OK");
				return null;
			}

			try
			{
				return await CognitiveService.Instance.DetectFaceAndEmotionsAsync(inputFile);

			}
			catch (Exception ex)
			{

				await DisplayAlert("Error", ex.Message, "OK");
				return null;
			}
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

			ShowActivityIndicator(true);

			// Upload image to azure blob storage
			azureImage = await AzureStorage.UploadImage(file);

			Image1.Source = ImageSource.FromStream(() => file.GetStream());
			this.BindingContext = await DetectFaceAndEmotionsAsync(azureImage.StorageUri);

			AddFaceButton.IsEnabled = true;
			ShowActivityIndicator(false);
		}

		private async void UploadPictureButton_Clicked(object sender, EventArgs e)
		{
			// AZURE IMAGES
			var modal = new ImageSelectionModalPage();
			await Navigation.PushModalAsync(modal);

			modal.SelectedItem += async (s, se) =>
			{
				azureImage = (ImageModel)s;
				Image1.Source = ImageSource.FromUri(new Uri(azureImage.StorageUri));

				ShowActivityIndicator(true);
				this.BindingContext = await DetectFaceAndEmotionsAsync(azureImage.StorageUri);

				AddFaceButton.IsEnabled = true;
				ShowActivityIndicator(false);
			};
		}

		private async void AddFaceButton_Clicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(NameEntry.Text)) return;
			
			ShowActivityIndicator(true);
			await CognitiveService.Instance.AddPersonFaceAsync(NameEntry.Text, azureImage.StorageUri);
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