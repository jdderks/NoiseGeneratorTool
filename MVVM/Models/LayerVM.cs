using MVVM.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Serialization;

namespace MVVM.Models
{
    public enum BlendModes
    {
        Additive = 0,
        Multiply = 1
    }

    [XmlInclude(typeof(SmoothNoiseLayerVM))]
    public class LayerVM : ViewModel
    {
        private string name;
        private float[][] bitmap;
        private int resolutionX;
        private int resolutionY;
        private byte opacity;

        private byte colorR;
        private byte colorG;
        private byte colorB;

        private int blendModeIndex = 0;

        private byte[,,] pixels;

        public LayerVM()
        {

        }

        #region Properties
        [XmlIgnore]
        public byte[,,] Pixels
        {
            get { return pixels;  }
            set
            {
                pixels = value;
                OnPropertyChanged(nameof(Pixels));
            }
        }

        public byte Opacity
        {
            get { return opacity; }
            set 
            { 
                opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }


        public int ResolutionY
        {
            get { return resolutionY; }
            set 
            {
                resolutionY = value;
                OnPropertyChanged(nameof(ResolutionY));
            }
        }
        public int ResolutionX
        {
            get { return resolutionX; }
            set 
            { 
                resolutionX = value;
                OnPropertyChanged(nameof(ResolutionX));
            }
        }
        [XmlIgnore]
        public float[][] Bitmap
        {
            get { return bitmap; }
            set
            {
                OnPropertyChanged(nameof(Bitmap));
                bitmap = value; 
            }
        }



        public int BlendModeIndex
        {
            get { return blendModeIndex; }
            set 
            {
                OnPropertyChanged(nameof(BlendModeIndex));
                blendModeIndex = value; 
            }
        }


        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public byte ColorR
        {
            get { return colorR; }
            set
            {
                colorR = value;
                OnPropertyChanged(nameof(ColorR));
            }
        }
        public byte ColorG
        {
            get { return colorG; }
            set
            {
                colorG = value;
                OnPropertyChanged(nameof(ColorG));
            }
        }
        public byte ColorB
        {
            get { return colorB; }
            set
            {
                colorB = value;
                OnPropertyChanged(nameof(ColorB));
            }
        }


        #endregion

        public virtual void CalculateLayerContent()
        {
            pixels = new byte[resolutionX, resolutionY, 4];

            for (int x = 0; x < resolutionX; x++)
            {
                for (int y = 0; y < resolutionY; y++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        pixels[x, y, 0] = colorB;
                        pixels[x, y, 1] = colorG;
                        pixels[x, y, 2] = colorR;

                        pixels[x, y, 3] = opacity;
                    }
                }
            }
        }

        public static ObservableCollection<LayerVM> Load(string _fileName)
        {
            return SerializeDeSerialize<ObservableCollection<LayerVM>>.FromFile(_fileName);
        }
    }


}
