using Microsoft.Win32;
using MVVM.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MVVM.Models
{


    class UploadOwnTextureLayerVM : LayerVM
    {
        #region properties
        public ICommand SetBitMapAsOwnImageCommand { get; }
        
        #endregion
        public UploadOwnTextureLayerVM()
        {
            SetBitMapAsOwnImageCommand = new Command(SetBitMapAsImageAction);
        }

        private void SetBitMapAsImageAction()
        {
            var newDialog = new OpenFileDialog() 
            {
                Filter = "Png Files ( *.png)|*.png"
            }; 
            if (newDialog.ShowDialog() == true)
            {
                Stream imageStreamSource = new FileStream(newDialog.FileName,FileMode.Open,FileAccess.Read,FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                ResolutionX = (int)bitmapSource.PixelWidth;
                ResolutionY = (int)bitmapSource.PixelHeight;

                Pixels = new byte[ResolutionX, ResolutionX, 4];

                int stride = bitmapSource.PixelWidth * 4;
                int size = bitmapSource.PixelHeight * stride;
                byte[] pixels = new byte[size];
                bitmapSource.CopyPixels(pixels, stride, 0);

                for (int x = 0; x < bitmapSource.PixelWidth; x++)
                {
                    for (int y = 0; y < bitmapSource.PixelHeight; y++)
                    {
                        
                        int index = y * stride + 4 * x;
                        Pixels[x, y, 0] = pixels[index];
                        Pixels[x, y, 1] = pixels[index + 1]; //Blue
                        Pixels[x, y, 2] = pixels[index + 2]; //Green
                        Pixels[x, y, 3] = pixels[index + 3]; //Red

                    }
                }

            }
        }
    }
}
