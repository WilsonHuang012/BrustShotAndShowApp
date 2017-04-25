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
using BrustShotAndShowApp.Droid.DependencyServices;
using BrustShotAndShowApp.Interfaces;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(GetFileService))]
namespace BrustShotAndShowApp.Droid.DependencyServices
{
    public class GetFileService : IGetFile
    {
        public string GetFile(string fileName)
        {
            var absolutePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;
            string folderPath = absolutePath + "/Camara";
            var filePath = System.IO.Path.Combine(folderPath, string.Format("{0}.jpg", fileName));
            return File.Exists(filePath) ? filePath : string.Empty;
        }
    }
}