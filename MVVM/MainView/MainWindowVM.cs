﻿using MVVM.General;
using MVVM.Models;
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
using System.Collections.ObjectModel;

namespace MVVM.MainView
{
    #region GenerationModesEnum
    enum GenerationModes
    {
        WhiteNoise = 0,
        SmoothedNoise = 1,
        PerlinNoise = 2
    }
    #endregion

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
        public ObservableCollection<LayerVM> layers; // = new ObservableCollection<Layer>();
        public LayerVM selectedLayer;

        #region Properties
        public GenerationModes mode { get; } = GenerationModes.WhiteNoise;
        
        public ObservableCollection<LayerVM> Layers
        {
            get { return layers; }
            set
            {
                layers = value;
                OnPropertyChanged(nameof(Layers));
            }
        }
        public LayerVM SelectedLayer
        {
            get { return selectedLayer; }
            set
            {
                selectedLayer = value;
                OnPropertyChanged(nameof(SelectedLayer));
            }
        }

        public int SelectedGenerationMode
        {
            get { return _selectedGenerationMode; }
            set
            {
                _selectedGenerationMode = value;
                OnPropertyChanged(nameof(SelectedGenerationMode));
            }
        }

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
        public ICommand GenerateCommand { get; }
        public ICommand ExportImageCommand { get; }
        public ICommand AddNewLayerCommand { get; }
        #endregion

        #region Constructor
        public MainWindowVM()
        {
            Seed = "0123456789";
            Text = "New layer";
            DisplayImage = new WriteableBitmap(
            resolution, resolution, 96, 96, PixelFormats.Bgra32, null);
            pixels = new byte[resolution, resolution, 4];

            Layers = new ObservableCollection<LayerVM>();
            Layers.Add(new LayerVM() {Name = "Newest layer"});
            Layers.Add(new LayerVM() {Name = "Very big layerrrrrrrrrrrrr"});
            Layers.Add(new LayerVM() {Name = "Newest layer"});
            Layers.Add(new SmoothNoiseLayerVM() { Name = "New Smooth Layer", Seed = 000000 });
            selectedLayer = layers[0];

            GenerateWhiteNoise();

            ExitCommand = new Command(ExitAction);
            GenerateCommand = new Command(GenerateAction);
            ExportImageCommand = new Command(ExportAction);
            AddNewLayerCommand = new Command(AddLayerAction);
        }
        #endregion

        #region Actions
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

        private void AddLayerAction()
        {
            LayerVM newLayer = new LayerVM() { Name = "New layer" };
            Layers.Add(newLayer);
            selectedLayer = Layers[layers.Count - 1];
        }
        #endregion

        #region ExportImage
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
        #endregion

        #region Image Generation
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
        #endregion

    }
}
