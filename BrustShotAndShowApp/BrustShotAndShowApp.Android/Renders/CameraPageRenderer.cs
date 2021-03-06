﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using BrustShotAndShowApp.Renders;
using BrustShotAndShowApp.Droid.Renders;
using System.Threading;
using System.IO;
using Android.Graphics;
using Android.Hardware;
using Xamarin.Forms.Platform.Android;
using BrustShotAndShowApp.Views;
using PCLStorage;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(CameraPage), typeof(CameraPageRenderer))]
namespace BrustShotAndShowApp.Droid.Renders
{
    public class CameraPageRenderer : PageRenderer, TextureView.ISurfaceTextureListener
    {
        global::Android.Hardware.Camera camera;
        global::Android.Widget.Button takePhotoButton;
        global::Android.Widget.Button toggleFlashButton;
        global::Android.Widget.Button switchCameraButton;
        global::Android.Views.View view;

        Activity activity;
        CameraFacing cameraType;
        TextureView textureView;
        SurfaceTexture surfaceTexture;
        App app = (App)Xamarin.Forms.Application.Current;
        bool flashOn;
        byte[] imageBytes;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                SetupUserInterface();
                SetupEventHandlers();
                AddView(view);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"			ERROR: ", ex.Message);
            }
        }

        void SetupUserInterface()
        {
            activity = this.Context as Activity;
            view = activity.LayoutInflater.Inflate(Resource.Layout.CameraLayout, this, false);
            cameraType = CameraFacing.Back;

            textureView = view.FindViewById<TextureView>(Resource.Id.textureView);
            textureView.SurfaceTextureListener = this;
        }

        void SetupEventHandlers()
        {
            takePhotoButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.takePhotoButton);
            takePhotoButton.Click += TakePhotoButtonTapped;

            switchCameraButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.switchCameraButton);
            switchCameraButton.Click += SwitchCameraButtonTapped;

            toggleFlashButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.toggleFlashButton);
            toggleFlashButton.Click += ToggleFlashButtonTapped;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            view.Measure(msw, msh);
            view.Layout(0, 0, r - l, b - t);
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {

        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            try
            {
                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);
                surfaceTexture = surface;

                camera.SetPreviewTexture(surface);
            }
            catch (Exception e)
            {

            }
            PrepareAndStartCamera();
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            camera.StopPreview();
            camera.SetPreviewCallback(null);
            camera.Release();

            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            PrepareAndStartCamera();
        }

        void PrepareAndStartCamera()
        {
            camera.StopPreview();

            var display = activity.WindowManager.DefaultDisplay;
            if (display.Rotation == SurfaceOrientation.Rotation0)
            {
                camera.SetDisplayOrientation(90);
            }

            if (display.Rotation == SurfaceOrientation.Rotation270)
            {
                camera.SetDisplayOrientation(180);
            }

            camera.StartPreview();
        }

        void ToggleFlashButtonTapped(object sender, EventArgs e)
        {
            flashOn = !flashOn;
            if (flashOn)
            {
                if (cameraType == CameraFacing.Back)
                {
                    toggleFlashButton.SetBackgroundResource(Resource.Drawable.FlashButton);
                    cameraType = CameraFacing.Back;

                    camera.StopPreview();
                    camera.Release();
                    camera = global::Android.Hardware.Camera.Open((int)cameraType);
                    var parameters = camera.GetParameters();
                    parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeTorch;
                    camera.SetParameters(parameters);
                    camera.SetPreviewTexture(surfaceTexture);
                    PrepareAndStartCamera();
                }
            }
            else
            {
                toggleFlashButton.SetBackgroundResource(Resource.Drawable.NoFlashButton);
                camera.StopPreview();
                camera.Release();

                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                var parameters = camera.GetParameters();
                parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeOff;
                camera.SetParameters(parameters);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        void SwitchCameraButtonTapped(object sender, EventArgs e)
        {
            if (cameraType == CameraFacing.Front)
            {
                cameraType = CameraFacing.Back;

                camera.StopPreview();
                camera.Release();
                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
            else
            {
                cameraType = CameraFacing.Front;

                camera.StopPreview();
                camera.Release();
                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        async void TakePhotoButtonTapped(object sender, EventArgs e)
        {
            var absolutePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;
            string folderPath = System.IO.Path.Combine(absolutePath, "Camera");
			Console.WriteLine($"folderPath = {folderPath}");
            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }
            //資料夾名稱
            string folderName = "Photos";

            #region PCL Storage
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var isFolderExist = await rootFolder.CheckExistsAsync(folderName);
            if (isFolderExist == ExistenceCheckResult.FolderExists)
            {
                IFolder existFolder = await rootFolder.GetFolderAsync(folderName);
                await existFolder.DeleteAsync();
            }
            IFolder folder = await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            #endregion
            
            for (int i = 0; i < 15; i++)
            {
                
                camera.StopPreview();
                var originalImageBitmap = textureView.Bitmap;
                string fileName = string.Format("{0}.jpg", i);
                try
                {
                	var originalImageFullPath = 
						System.IO.Path.Combine(folderPath, string.Format("{0}.jpg", i));
	                using (var originalImageStream = new FileStream(originalImageFullPath, FileMode.OpenOrCreate))
				    {
				        await originalImageBitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 50, originalImageStream);
				    }
				    var originalImageBytes = File.ReadAllBytes(originalImageFullPath);
				    
                    var outputFilePath = System.IO.Path.Combine(folderPath, $"{i}_compress.jpg");
					//各平台壓縮方式參考
					//  https://github.com/xamarin/xamarin-forms-samples/blob/master/XamFormsImageResize/XamFormsImageResize/ImageResizer.cs
					var newImageBytes = await ResizeImageAndroid(originalImageBytes,
													originalImageBitmap.Width / 2,
													originalImageBitmap.Height / 2);

					File.WriteAllBytes(outputFilePath, newImageBytes);

                    #region PCL File
                    IFile PCLFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    using (var PCLFilestream = await PCLFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                    {
                        //write byte array to stream
                        await PCLFilestream.WriteAsync(newImageBytes, 0, newImageBytes.Length);
                        System.Diagnostics.Debug.WriteLine(PCLFilestream.Length);
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(@"				", ex.Message);
                }
                camera.StartPreview();
                Thread.Sleep(100);
            }
            MainPage page = new MainPage();
            MessagingCenter.Send<MainPage>(page, "TakePhoto");

        }

        /// <summary>
        /// Copy Stream
        /// </summary>
        /// <param name="input">
        /// <param name="output">
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }        
        
        public async Task<byte[]> ResizeImageAndroid (byte[] imageData, float width, float height)
		{
			// Load the bitmap
			Bitmap originalImage = BitmapFactory.DecodeByteArray (imageData, 0, imageData.Length);
			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

			using (MemoryStream ms = new MemoryStream())
			{
				await resizedImage.CompressAsync(Bitmap.CompressFormat.Jpeg, 50, ms);
				return ms.ToArray ();
			}
		}

    }
}