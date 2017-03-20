using BrustShotAndShowApp.CustomRenderPage;
using BrustShotAndShowApp.Interfaces;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BrustShotAndShowApp.Views
{
    public partial class MainPage : ContentPage
    {
        private double num = 0.0f;
        private int moveindex = 30;
        private string androidPath = string.Empty;

        public MainPage()
        {
            InitializeComponent();

            takePhotoButton.Clicked += TakePhotoButton_Clicked;

            CrossDeviceMotion.Current.SensorValueChanged += Current_SensorValueChanged;
        }

        private void Current_SensorValueChanged(object sender, SensorValueChangedEventArgs e)
        {
            switch (e.SensorType)
            {
                case MotionSensorType.Accelerometer:
                    num = ((MotionVector)e.Value).X;

                    if (Device.OS == TargetPlatform.iOS)
                    {

                        GlobalViewModel.PhotosViewModel.Number = Math.Round(num, 2);
                        GlobalViewModel.PhotosViewModel.Index = GetIndex(GlobalViewModel.PhotosViewModel.Number);
                        if (GlobalViewModel.PhotosViewModel.ImageList.Count > 0)
                        {
                            if (num < 0)
                            {
                                if (moveindex > 0)
                                {

                                    image1.Source = GlobalViewModel.PhotosViewModel.ImageList[moveindex];
                                    moveindex = moveindex - 1;
                                }


                            }
                            else if (num > 0)
                            {
                                if (moveindex < GlobalViewModel.PhotosViewModel.ImageList.Count - 1)
                                {
                                    image1.Source = GlobalViewModel.PhotosViewModel.ImageList[moveindex];
                                    moveindex = moveindex + 1;
                                }

                            }
                            else
                            {
                                double number = Math.Round(num, 2);
                                int index = GetIndex(number);
                                image1.Source = DependencyService.Get<IGetFile>().GetFile(Convert.ToString(index));
                            }

                        }
                    }
                    else if (Device.OS == TargetPlatform.Android)
                    {
                        double number = Math.Round(num, 2);
                        int index = GetAndroidIndex(number);
                        image1.Source = DependencyService.Get<IGetFile>().GetFile(Convert.ToString(index));
                    }

                    break;
            }
        }

        private int GetAndroidIndex(double number)
        {
            if (number == -0.40f)
            {
                return 0;
            }
            else if (number == 0.40f)
            {
                return moveindex - 1;
            }
            else
            {
                return Convert.ToInt32((number * 50) + 20);
            }
        }

        /// <summary>
        /// y = 50x + 20
        /// </summary>
        /// <param name="number">x</param>
        /// <returns>y</returns>
        private int GetIndex(double number)
        {
            if (number == -0.40f)
            {
                return 0;
            }
            else if (number == 0.40f)
            {
                return GlobalViewModel.PhotosViewModel.ImageList.Count - 1;
            }
            else
            {
                return Convert.ToInt32((number * 50) + 20);
            }

        }

        async void TakePhotoButton_Clicked(object sender, EventArgs e)
        {
            CrossDeviceMotion.Current.Stop(MotionSensorType.Accelerometer);
            await Navigation.PushAsync(new CameraPage());
        }

        protected override void OnAppearing()
        {
            CrossDeviceMotion.Current.Start(MotionSensorType.Accelerometer);
        }

        private ImageSource GetStreamByIndex(int index)
        {
            if (GlobalViewModel.PhotosViewModel.ImageList.Count > 0 && index >= GlobalViewModel.PhotosViewModel.ImageList.Count)
            {
                return GlobalViewModel.PhotosViewModel.ImageList[GlobalViewModel.PhotosViewModel.ImageList.Count - 1];
            }
            else if (GlobalViewModel.PhotosViewModel.ImageList.Count == 0)
            {
                return null;
            }
            else
            {
                return GlobalViewModel.PhotosViewModel.ImageList[index];
            }
        }
    }
}
