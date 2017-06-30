using BrustShotAndShowApp.ViewModels;
using BrustShotAndShowApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace BrustShotAndShowApp
{
    public static class GlobalViewModel
    {
        private static PhotosViewModel photosViewModel;
        public static PhotosViewModel PhotosViewModel => photosViewModel ?? (photosViewModel = new PhotosViewModel());
    }

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPageCS());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
