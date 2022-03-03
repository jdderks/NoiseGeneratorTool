using MVVM.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MVVM.MainView
{
    enum GenerationMode
    {
        WhiteNoise = 0,
        SmoothNoise = 1
    }


    class MainWindowVM : ViewModel
    {
        private string text;
        private string seed;
        private WriteableBitmap img;
        private byte[,,] pixels;
        float[][] bitmap;
        private int resolution = 512;
        private bool randomizeSeedOnGenerate = false;

        public GenerationMode mode = GenerationMode.WhiteNoise;

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
            switch (mode)
            {
                case GenerationMode.WhiteNoise:
                    GenerateWhiteNoise();
                    break;
                case GenerationMode.SmoothNoise:
                    GenerateWhiteNoise();
                    GenerateSmoothWhiteNoise();
                    break;
                default:
                    break;
            }
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
