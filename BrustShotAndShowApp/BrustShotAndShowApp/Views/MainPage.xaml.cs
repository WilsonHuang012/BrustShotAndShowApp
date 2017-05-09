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
using PCLStorage;

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
                    PhotoSlider.Maximum = 29;
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
                    string fileName = string.Format("{0}_compress.jpg", index);
                    
                    image1.Source = "/data/user/0/BrustShotAndShowApp.Android/files/Camera/" + fileName;
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
				//GetImage(fileName);
				image1.Source = "/storage/emulated/0/DCIM/Camera/0_compress.jpg";
            }
        }

        private async void GetImage(string fileName)
        {
            #region PCL Storage

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var isFolderExist = await rootFolder.CheckExistsAsync("Camera");
            if (isFolderExist == ExistenceCheckResult.FolderExists)
            {
                IFolder folder = await rootFolder.GetFolderAsync("Camera");
                var isFileExist = await folder.CheckExistsAsync(fileName);
                if (isFileExist == ExistenceCheckResult.FileExists)
                {
                    IFile file = await folder.GetFileAsync(fileName);
                   
                }
            }
            #endregion
        }

    }
}
