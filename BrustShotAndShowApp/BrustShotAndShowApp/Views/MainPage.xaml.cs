using BrustShotAndShowApp.Renders;
using BrustShotAndShowApp.Interfaces;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrustShotAndShowApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private double StepValue;

        public MainPage()
        {
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    PhotoSlider.Maximum = 29;
                    break;
                case Device.Android:
                    PhotoSlider.Maximum = 1;
                    break;
            }

            takePhotoButton.Clicked += TakePhotoButton_Clicked;
            PhotoSlider.ValueChanged += PhotoSlider_ValueChanged;
            StepValue = 1.0;
        }

        private void PhotoSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / StepValue);
            PhotoSlider.Value = newStep * StepValue;
            if (Device.RuntimePlatform == Device.iOS)
            {
                if (GlobalViewModel.PhotosViewModel.ImageList.Count > 0)
                {
                    int index = Convert.ToInt32(PhotoSlider.Value);
                    if (index > -1)
                    {
                        image1.Source = GlobalViewModel.PhotosViewModel.ImageList[index];
                    }
                }
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                int index = Convert.ToInt32(PhotoSlider.Value);
                if (index > -1)
                {
                    image1.Source = ImageSource.FromFile(DependencyService.Get<IGetFile>().GetFile(index.ToString()));
                }
            }
        }

        async void TakePhotoButton_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Subscribe<MainPage>(this, "TakePhoto", async (s) => {
                await Navigation.PopAsync(true);
            });

            await Navigation.PushAsync(new CameraPage());
        }

        ~MainPage()
        {
            MessagingCenter.Unsubscribe<MainPage>(this, "TakePhoto");
        }

        protected override void OnAppearing()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                if (GlobalViewModel.PhotosViewModel.ImageList.Count > 0)
                {
                    image1.Source = GlobalViewModel.PhotosViewModel.ImageList[0];
                }
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                image1.Source = ImageSource.FromFile(DependencyService.Get<IGetFile>().GetFile("0"));
            }
        }

    }
}
