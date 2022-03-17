using MVVM.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Win32;

namespace MVVM.MainView
{
    enum GenerationModes
    {
        WhiteNoise = 0,
        SmoothedNoise = 1,
        PerlinNoise = 2
    }


    class MainWindowVM : ViewModel
    {
        private string text;
        private string seed;
        private WriteableBitmap img;
        private byte[,,] pixels;
        float[][] bitmap;
        private int resolution = 512;
        private int _selectedGenerationMode;

        static MainWindowVM()
        {
            AvailableGenerationModes = new List<string>(Enum.GetNames(typeof(GenerationModes)));
        }

        public static List<string> AvailableGenerationModes { get; }


        public int SelectedGenerationMode
        {
            get { return _selectedGenerationMode; }
            set
            {
                _selectedGenerationMode = value;
                OnPropertyChanged(nameof(SelectedGenerationMode));
            }
        }


        public GenerationModes mode { get; } = GenerationModes.WhiteNoise;

        public string Seed
        {
            get { return seed; }
            set
            {
                seed = value;
                OnPropertyChanged(nameof(Seed));
            }
        }

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public WriteableBitmap DisplayImage
        {
            get { return img; }
            set
            {
                img = value;
                OnPropertyChanged(nameof(DisplayImage));
            }
        }

        public ICommand ExitCommand { get; }
        public ICommand ConfigCommand { get; }

        public ICommand GenerateCommand { get; }

        public ICommand ExportImageCommand { get; }

        public MainWindowVM()
        {
            Seed = "0123456789";
            Text = "New file";
            DisplayImage = new WriteableBitmap(
            resolution, resolution, 96, 96, PixelFormats.Bgra32, null);
            pixels = new byte[resolution, resolution, 4];

            GenerateWhiteNoise();

            ExitCommand = new Command(ExitAction);
            ConfigCommand = new Command(ConfigAction);
            GenerateCommand = new Command(GenerateAction);
            ExportImageCommand = new Command(ExportAction);
        }

        private void SetModeAction()
        {

        }

        private void ConfigAction()
        {

        }

        private void ExitAction()
        {
            Application.Current.Shutdown();
        }

        private void GenerateAction()
        {
            switch ((GenerationModes)SelectedGenerationMode)
            {
                case GenerationModes.WhiteNoise:
                    GenerateWhiteNoise();
                    break;
                case GenerationModes.SmoothedNoise:
                    GenerateWhiteNoise();
                    GenerateSmoothWhiteNoise();
                    break;
                default:
                    break;
            }
        }

        private void ExportAction()
        {
            SaveImage();
        }

        private void SaveImage()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog()
                {
                    Filter = "Image Files ( *.png, *.bmp, *.jpg)|*.png;*.bmp;*.jpg"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)ConvertWriteableBitmapToBitmapImage(DisplayImage)));
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(stream);
                    }
                }
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            //From https://stackoverflow.com/questions/14161665/how-do-i-convert-a-writeablebitmap-object-to-a-bitmapimage-object-in-wpf
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        private void GenerateWhiteNoise()
        {
            bitmap = Perlin.Noise.GenerateWhiteNoise(resolution, resolution, Seed);

            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        pixels[x, y, 3] = 255; //3 is alpha channel
                        
                        pixels[x, y, 0] = (byte)(bitmap[x][y]*255); //Blue
                        pixels[x, y, 1] = (byte)(bitmap[x][y]*255); //Green
                        pixels[x, y, 2] = (byte)(bitmap[x][y]*255); //Red
                    }
                }
            }

            UpdateImageRect();
        }

        private void GenerateSmoothWhiteNoise()
        {
            bitmap = Perlin.Noise.GenerateWhiteNoise(resolution, resolution, Seed);
            bitmap = Perlin.Noise.GenerateSmoothNoise(bitmap, 3);

            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        pixels[x, y, 3] = 255; //3 is alpha channel

                        pixels[x, y, 0] = (byte)(bitmap[x][y] * 255); //Blue
                        pixels[x, y, 1] = (byte)(bitmap[x][y] * 255); //Green
                        pixels[x, y, 2] = (byte)(bitmap[x][y] * 255); //Red
                    }
                }
            }
            UpdateImageRect();
        }


        private void UpdateImageRect()
        {
            byte[] pixels1d = new byte[resolution * resolution * 4];
            int index = 0;
            for (int row = 0; row < resolution; row++)
            {
                for (int col = 0; col < resolution; col++)
                {
                    for (int i = 0; i < 4; i++)
                        pixels1d[index++] = pixels[row, col, i];
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, resolution, resolution);
            int stride = 4 * resolution;
            DisplayImage.WritePixels(rect, pixels1d, stride, 0);
        }
    }
}
