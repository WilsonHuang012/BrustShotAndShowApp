using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BrustShotAndShowApp.ViewModels
{
    public class PhotosViewModel : INotifyPropertyChanged
    {
        private double number = 0.0f;

        public List<ImageSource> ImageList = new List<ImageSource>();

        public double Number
        {
            get
            {
                return number;
            }
            set
            {
                if (value != number)
                {
                    if (value < -0.40f)
                    {
                        number = -0.40f;
                    }
                    else if (value > 0.40f)
                    {
                        number = 0.40f;
                    }
                    else
                    {
                        number = Math.Round(value, 2);
                    }
                    OnPropertyChanged(nameof(Number));
                }
            }
        }

        private ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            set { image = value; }
        }

        private int index = 0;

        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged(nameof(Index));
            }
        }

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
