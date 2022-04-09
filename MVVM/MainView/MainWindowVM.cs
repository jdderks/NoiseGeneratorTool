using MVVM.General;
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
using System.Xml.Serialization;

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
        private WriteableBitmap img;
        private byte[,,] pixels;
        float[][] bitmap;
        private int resolution = 512;
        private string projectName;
        //private int _selectedGenerationMode;

        private string statusText;

        static MainWindowVM()
        {
            AvailableGenerationModes = new List<string>(Enum.GetNames(typeof(GenerationModes)));
        }

        public static List<string> AvailableGenerationModes { get; }
        public ObservableCollection<LayerVM> layers; // = new ObservableCollection<Layer>();
        public LayerVM selectedLayer;

        #region Properties
        //public GenerationModes mode { get; } = GenerationModes.WhiteNoise;

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

        public WriteableBitmap DisplayImage
        {
            get { return img; }
            set
            {
                img = value;
                OnPropertyChanged(nameof(DisplayImage));
            }
        }

        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }

        }
        [XmlIgnore] public ICommand ExitCommand { get; }
        [XmlIgnore] public ICommand GenerateCommand { get; }
        [XmlIgnore] public ICommand SaveProjectCommand { get; }
        [XmlIgnore] public ICommand SaveAsProjectCommand { get; }
        [XmlIgnore] public ICommand LoadProjectCommand { get; }
        [XmlIgnore] public ICommand ExportImageCommand { get; }
        [XmlIgnore] public ICommand AddNewLayerCommand { get; }
        [XmlIgnore] public ICommand AddNewSmoothNoiseLayerCommand { get; }
        [XmlIgnore] public ICommand RemoveLayerCommand { get; }
        #endregion
        #region Constructor
        public MainWindowVM()
        {
            DisplayImage = new WriteableBitmap(
            resolution, resolution, 96, 96, PixelFormats.Bgra32, null);
            pixels = new byte[resolution, resolution, 4];

            Layers = new ObservableCollection<LayerVM>();
            //Layers.Add(new LayerVM() {Name = "Newest layer"});
            //Layers.Add(new LayerVM() {Name = "Very big layerrrrrrrrrrrrr"});
            //Layers.Add(new LayerVM() {Name = "Newest layer"});
            //Layers.Add(new SmoothNoiseLayerVM() { Name = "New Smooth Layer", Seed = 000000 });
            //selectedLayer = layers[0];

            //GenerateWhiteNoise();

            ExitCommand = new Command(ExitAction);
            GenerateCommand = new Command(GenerateAction);
            SaveProjectCommand = new Command(SaveProjectAction, () => !string.IsNullOrEmpty(projectName));
            SaveAsProjectCommand = new Command(SaveAsProjectAction);
            LoadProjectCommand = new Command(LoadProjectAction);
            ExportImageCommand = new Command(ExportAction);
            AddNewLayerCommand = new Command(AddLayerAction);
            AddNewSmoothNoiseLayerCommand = new Command(AddSmoothLayerAction);
            RemoveLayerCommand = new Command(RemoveSelectedLayerAction);

            GenerateAction();
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
            pixels = new byte[resolution, resolution, 4]; //Reset the pixels
            CalculateLayers();
            UpdateImageRect();
        }
        private void SaveProjectAction()
        {
            SerializeDeSerialize<ObservableCollection<LayerVM>>.ToFile(projectName,  Layers);
        }

        private void SaveAsProjectAction()
        {
            var saveAsDialog = new SaveFileDialog()
            {
                Filter = "Project files (*.jtb)|*.jtb;"
            }; 
            if (saveAsDialog.ShowDialog() == true)
            {
                projectName = saveAsDialog.FileName;
                SaveProjectAction();
            }
        }

        private void LoadProjectAction()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Project files (*.jtb)|*.jtb;"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                projectName = openFileDialog.FileName;
                Layers = LayerVM.Load(projectName);
            }
        }

        private void ExportAction()
        {
            StatusText = "Exported image";
            SaveImage();
        }

        private void AddLayerAction()
        {
            StatusText = "Added new solid colour layer.";
            LayerVM newLayer = new LayerVM()
            {
                Name = "New solid color layer",
                ResolutionX = 512,
                ResolutionY = 512,
                Opacity = 255,
                ColorR = 255,
                ColorG = 255,
                ColorB = 255
            };
            newLayer.CalculateLayerContent();
            Layers.Add(newLayer);
            selectedLayer = Layers[layers.Count - 1];
        }
        private void AddSmoothLayerAction()
        {
            StatusText = "Added new smooth noise layer.";
            SmoothNoiseLayerVM newLayer = new SmoothNoiseLayerVM()
            {
                Name = "New noise layer",
                ResolutionX = 512,
                ResolutionY = 512,
                Opacity = 255,
                ColorR = 255,
                ColorG = 255,
                ColorB = 255,
                Seed = 1234,
                IsSmoothed = false
            };
            newLayer.CalculateLayerContent();
            layers.Add(newLayer);
            selectedLayer = Layers[layers.Count - 1];
        }

        private void RemoveSelectedLayerAction()
        {
            StatusText = "Removed selected layer.";
            layers.Remove(selectedLayer);
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
        #endregion
        #region Noise region

        //private void GenerateWhiteNoise(string seed)
        //{
        //    bitmap = Perlin.Noise.GenerateWhiteNoise(resolution, resolution, seed);

        //    for (int x = 0; x < resolution; x++)
        //    {
        //        for (int y = 0; y < resolution; y++)
        //        {
        //            for (int i = 0; i < 3; i++)
        //            {
        //                pixels[x, y, 3] = 255; //3 is alpha channel

        //                pixels[x, y, 0] = (byte)(bitmap[x][y] * 255); //Blue
        //                pixels[x, y, 1] = (byte)(bitmap[x][y] * 255); //Green
        //                pixels[x, y, 2] = (byte)(bitmap[x][y] * 255); //Red
        //            }
        //        }
        //    }
        //}

        //private void GenerateSmoothWhiteNoise(string seed)
        //{
        //    bitmap = Perlin.Noise.GenerateWhiteNoise(resolution, resolution, seed);
        //    bitmap = Perlin.Noise.GenerateSmoothNoise(bitmap, 3);

        //    for (int x = 0; x < resolution; x++)
        //    {
        //        for (int y = 0; y < resolution; y++)
        //        {
        //            for (int i = 0; i < 3; i++)
        //            {
        //                pixels[x, y, 3] = 255; //3 is alpha channel

        //                pixels[x, y, 0] = (byte)(bitmap[x][y] * 255); //Blue
        //                pixels[x, y, 1] = (byte)(bitmap[x][y] * 255); //Green
        //                pixels[x, y, 2] = (byte)(bitmap[x][y] * 255); //Red
        //            }
        //        }
        //    }
        //    UpdateImageRect();
        //}
        #endregion
        #region Layermath
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

        private void CalculateLayers()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].CalculateLayerContent();

                switch (layers[i].BlendModeIndex)
                {
                    case 0: //Additive
                        for (int x = 0; x < layers[i].Pixels.GetLength(0); x++)
                        {
                            for (int y = 0; y < layers[i].Pixels.GetLength(1); y++)
                            {
                                pixels[x, y, 0] += layers[i].Pixels[x, y, 0];
                                pixels[x, y, 1] += layers[i].Pixels[x, y, 1];
                                pixels[x, y, 2] += layers[i].Pixels[x, y, 2];

                                pixels[x, y, 3] += layers[i].Pixels[x, y, 3];

                                if (pixels[x, y, 0] > 255)
                                {
                                    pixels[x, y, 0] = 255;
                                }
                                if (pixels[x, y, 1] > 255)
                                {
                                    pixels[x, y, 1] = 255;
                                }
                                if (pixels[x, y, 2] > 255)
                                {
                                    pixels[x, y, 2] = 255;
                                }
                                if (pixels[x, y, 2] > 255)
                                {
                                    pixels[x, y, 2] = 255;
                                }
                                if (pixels[x, y, 3] > 255)
                                {
                                    pixels[x, y, 3] = 255;
                                }
                            }
                        }
                        break;
                    case 1: //Multiply
                        for (int x = 0; x < layers.Count; x++)
                        {

                            //for (int y = 0; y < layers.Count; y++)
                            //{
                            //    float multiplier = layers[i].Bitmap[x][y] / 255;
                            //    bitmap[x][y] *= multiplier;
                            //}
                        }

                        break;
                    default:
                        break;
                }


            }
        }
        #endregion

        
    }
}
