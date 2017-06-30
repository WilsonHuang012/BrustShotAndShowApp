using BrustShotAndShowApp.Renders;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace BrustShotAndShowApp.Views
{
    public class MainPageCS : ContentPage
    {
        RelativeLayout layout = new RelativeLayout()
        {
            HorizontalOptions = LayoutOptions.CenterAndExpand
        };

        Slider PhotoSlider = new Slider();
        private double StepValue = 1.0f;
        public MainPageCS()
        {

            #region Xaml CS

            
            PhotoSlider.Minimum = 0.0;
            PhotoSlider.Maximum = 14.0;
            PhotoSlider.Value = 0.0;
            PhotoSlider.ValueChanged += PhotoSlider_ValueChanged;

            Button TakePhotoButton = new Button();
            TakePhotoButton.Text = "Take Photos";
            TakePhotoButton.Clicked += TakePhotoButton_Clicked; ;

            //List<Image> imageList = new List<Image>();
            //for (int i = 0; i < 15; i++)
            //{
            //    Image image = new Image();
            //    imageList.Add(image);
            //}

            #region Add Image to RelativeLayout
            //for (int i = 0; i < imageList.Count; i++)
            //{
            //    layout.Children.Add(imageList[i],
            //       //xConstraint
            //       null,
            //       //yConstraint
            //       null,
            //       //widthConstraint
            //       Constraint.RelativeToParent((parent) =>
            //       {
            //           return parent.Width;
            //       }),
            //       //heightConstraint
            //       Constraint.RelativeToParent((parent) =>
            //       {
            //           return parent.Height * 0.8;
            //       })
            //   );
            //}
            #endregion

            #region Add Slider to RelativeLayout
            layout.Children.Add(PhotoSlider,
                //xConstraint
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * 0.20;
                }),
                //yConstraint
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height - 75;
                }),
                //widthConstraint
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * 0.8;
                })
            );
            #endregion

            #region Add Button to RelativeLayout
            layout.Children.Add(TakePhotoButton,
                //xConstraint
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * 0.20;
                }),
                //yConstraint
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height - 45;
                })

            );
            #endregion


            #endregion


        }

        private void PhotoSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / StepValue);
            PhotoSlider.Value = newStep * StepValue;
            int index = Convert.ToInt32(PhotoSlider.Value);
            if (index > -1)
            {
                
            }
        }

        private async void TakePhotoButton_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Unsubscribe<MainPageCS>(this, "TakePhoto");
            MessagingCenter.Subscribe<MainPageCS>(this, "TakePhoto", async (s) =>
            {
                await Navigation.PopAsync(true);
            });
            await Navigation.PushAsync(new CameraPage());
        }

        protected override void OnAppearing()
        {
            RemovePhotos();
            AddPhotos();
        }

        private void RemovePhotos()
        {
            for (int i = 0; i < layout.Children.Count; i++)
            {
                if (layout.Children[i] is Image)
                {
                    layout.Children.Remove(layout.Children[i]);
                }
            }
        }
        List<Image> imageList = new List<Image>();
        private async void AddPhotos()
        {
            string folderName = "Photos";
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var isFolderExist = await rootFolder.CheckExistsAsync(folderName);
            if (isFolderExist == ExistenceCheckResult.FolderExists)
            {
                IFolder folder = await rootFolder.GetFolderAsync(folderName);
                IList<IFile> photos = await folder.GetFilesAsync();
                for (int i = 0; i < photos.Count; i++)
                {
                    bool first = false;
                    if (i == 0)
                    {
                        first = true;
                    }
                    else
                    {
                        first = false;
                    }
                    Image image = new Image()
                    {
                        Source = photos[i].Path,
                        IsVisible = first
                    };
                    layout.Children.Add(image,
                        //xConstraint
                        null,
                        //yConstraint
                        null,
                        //widthConstraint
                        Constraint.RelativeToParent((parent) =>
                        {
                            return parent.Width;
                        }),
                        //heightConstraint
                        Constraint.RelativeToParent((parent) =>
                        {
                            return parent.Height * 0.8;
                        })
                    );
                    imageList.Add(image);
                }
            }
            Content = layout;
        }
    }
}