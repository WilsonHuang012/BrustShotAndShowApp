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
using BrustShotAndShowApp.Droid;
using Java.IO;

[assembly: Xamarin.Forms.Dependency(typeof(GetFileImplementation))]
namespace BrustShotAndShowApp.Droid
{
    public class GetFileImplementation : IGetFile
    {
        public GetFileImplementation() { }
        public string GetFile(string fileName)
        {
            var absolutePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;
            var folderPath = absolutePath + "/Camera";
            var filePath = System.IO.Path.Combine(folderPath, string.Format("{0}.jpg", fileName));
            File file = new File(filePath);
            if (file.Exists())
            {
                return file.Path;
            }
            else
            {
                return string.Empty;
            }
        }

      
    }
}