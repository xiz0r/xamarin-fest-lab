using Xamarin.Forms;

namespace Cognitive.Services.Lab
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void FaceDetection_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new DetectPage());
		}

		private async void FaceIdentification_Clicked(object sender, System.EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(CognitiveService.Instance?.PersonGroupId))
			{
				await Navigation.PushAsync(new IdentifyPage());
			}
			else
			{
				await DisplayAlert("", "No faces found in the current group", "Close");
			}
		}

		protected override void OnAppearing()
		{
			imgXamarinFest.Source = ImageSource.FromResource("Cognitive.Services.Lab.Resources.xamarinfest.png");
		}
	}
}