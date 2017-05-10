using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BrustShotAndShowApp.Interfaces;
using BrustShotAndShowApp.Droid.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(GetPathService))]
namespace BrustShotAndShowApp.Droid.DependencyServices
{
    public class GetPathService : IGetPath
    {
        public string GetPath()
        {
            var absolutePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;
            string folderPath = System.IO.Path.Combine(absolutePath, "Camera");
            return folderPath;
        }
    }
}