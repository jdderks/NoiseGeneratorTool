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

        public string ProjectName
        {
            get { return projectName; }
            set
            {
                projectName = value;
                OnPropertyChanged(nameof(ProjectName));
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title
        {
            get
            {
                string fileName = Path.GetFileNameWithoutExtension(projectName);
                return string.IsNullOrEmpty(projectName) ? "JTB 0.30 - New Project" : "JTB 0.30 - " + fileName;
            }
        }

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
        public ICommand ExitCommand { get; }
        public ICommand GenerateCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand SaveAsProjectCommand { get; }
        public ICommand LoadProjectCommand { get; }
        public ICommand ExportImageCommand { get; }
        public ICommand AddNewLayerCommand { get; }
        public ICommand AddNewSmoothNoiseLayerCommand { get; }
        public ICommand AddNewOwnImageLayerCommand { get; }
        public ICommand RemoveLayerCommand { get; }
        #endregion
        #region Constructor
        public MainWindowVM()
        {
            DisplayImage = new WriteableBitmap(
            resolution, resolution, 96, 96, PixelFormats.Bgra32, null);
            pixels = new byte[resolution, resolution, 4];

            Layers = new ObservableCollection<LayerVM>();

            ExitCommand = new Command(ExitAction);
            GenerateCommand = new Command(GenerateAction);
            SaveProjectCommand = new Command(SaveProjectAction, () => !string.IsNullOrEmpty(projectName));
            SaveAsProjectCommand = new Command(SaveAsProjectAction);
            LoadProjectCommand = new Command(LoadProjectAction);
            ExportImageCommand = new Command(ExportAction);
            AddNewLayerCommand = new Command(AddSolidColorLayerAction);
            AddNewSmoothNoiseLayerCommand = new Command(AddSmoothLayerAction);
            AddNewOwnImageLayerCommand = new Command(AddnewOwnImageLayerAction);
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
            for (int i = 0; i < Layers.Count; i++)
            {
                Layers[i].CalculateLayerContent();
            }
            CalculateLayers();
            UpdateImageRect();
        }
        private void SaveProjectAction()
        {
            SerializeDeSerialize<ObservableCollection<LayerVM>>.ToFile(ProjectName, Layers);
        }

        private void SaveAsProjectAction()
        {
            var saveAsDialog = new SaveFileDialog()
            {
                Filter = "Project files (*.jtb)|*.jtb;"
            };
            if (saveAsDialog.ShowDialog() == true)
            {
                ProjectName = saveAsDialog.FileName;
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
                ProjectName = openFileDialog.FileName;
                Layers = LayerVM.Load(ProjectName);
            }
            GenerateAction();
        }

        private void ExportAction()
        {
            StatusText = "Exported image";
            SaveImage();
        }

        private void AddSolidColorLayerAction()
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
        private void AddnewOwnImageLayerAction()
        {
            StatusText = "Added new own image layer.";
            UploadOwnTextureLayerVM newLayer = new UploadOwnTextureLayerVM()
            {
                Name = "New own image layer",
                ResolutionX = 512,
                ResolutionY = 512,
                Opacity = 255,
                ColorR = 255,
                ColorG = 255,
                ColorB = 255
            };
            //newLayer.CalculateLayerContent();
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
            LayerVM upperLayer = default;
            //Go through layers backwards to lay them on top of eachother
            
            for (int i = Layers.Count - 1; i > 0; i--)
            {
                LayerVM currentLayer = layers[i];
                upperLayer = layers[i - 1];

                for (int x = 0; x < Layers[i].Pixels.GetLength(0); x++)
                {
                    for (int y = 0; y < Layers[i].Pixels.GetLength(1); y++)
                    {
                        System.Drawing.Color currentColor = System.Drawing.Color.FromArgb(
                            currentLayer.Pixels[x, y, 3],  //alpha
                            currentLayer.Pixels[x, y, 2],  //red
                            currentLayer.Pixels[x, y, 1],  //Green
                            currentLayer.Pixels[x, y, 0]); //blue

                        System.Drawing.Color upperColor = System.Drawing.Color.FromArgb(
                            upperLayer.Pixels[x, y, 3],  //alpha
                            upperLayer.Pixels[x, y, 2],  //red
                            upperLayer.Pixels[x, y, 1],  //Green
                            upperLayer.Pixels[x, y, 0]); //blue

                        System.Drawing.Color blendedColor = BlendColor(upperColor, currentColor);

                        upperLayer.Pixels[x, y, 3] = blendedColor.A;
                        upperLayer.Pixels[x, y, 2] = blendedColor.R;
                        upperLayer.Pixels[x, y, 1] = blendedColor.G;
                        upperLayer.Pixels[x, y, 0] = blendedColor.B;

                        //pixels[x, y, 3] = blendedColor.A;
                        //pixels[x, y, 2] = blendedColor.R;
                        //pixels[x, y, 1] = blendedColor.G;
                        //pixels[x, y, 0] = blendedColor.B;
                    }
                }
                //When at last element of loop
                if (i == 1)
                {
                    pixels = upperLayer.Pixels;
                }
            }
        }
        #endregion
        public System.Drawing.Color BlendColor(System.Drawing.Color fg, System.Drawing.Color bg)
        {
            //Set color to 0 -> 1 values instead of 0 -> 255
            double fg_blue =  (double)(fg.B / (double)255);
            double fg_red =   (double)(fg.R / (double)255);
            double fg_green = (double)(fg.G / (double)255);
            double fg_alpha = (double)(fg.A / (double)255);

            double bg_blue =  (double)(bg.B / (double)255);
            double bg_red =   (double)(bg.R / (double)255);
            double bg_green = (double)(bg.G / (double)255);
            double bg_alpha = (double)(bg.A / (double)255);

            double al = 1 - (1 - fg_alpha) * (1 - bg_alpha);

            System.Drawing.Color r = System.Drawing.Color.FromArgb(0, 0, 0, 0);

            //Check whether or not it's fully transparent
            if ((double)(al) < 1.0e-6) 
            {
                return r;
            }

            //The math accelarates
            double red =    fg_red   * fg_alpha / al + bg_red   * bg_alpha * (1 - fg_alpha) / al;
            double green =  fg_green * fg_alpha / al + bg_green * bg_alpha * (1 - fg_alpha) / al;
            double blue =   fg_blue  * fg_alpha / al + bg_blue  * bg_alpha * (1 - fg_alpha) / al;

            //Set color to 0 -> 255 again
            int redoutput =   (int)(red * 255);
            int greenoutput = (int)(green * 255);
            int blueoutput =  (int)(blue * 255);


            r = System.Drawing.Color.FromArgb(
                (int)(al * 255),
                (int)(red * 255),
                (int)(green * 255),
                (int)(blue * 255)
                );

            return r;
        }
    }
}



