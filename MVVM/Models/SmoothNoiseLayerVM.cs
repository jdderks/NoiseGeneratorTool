using System;
using System.Collections.Generic;
using System.Text;

namespace MVVM.Models
{
    public class SmoothNoiseLayerVM : LayerVM
    {
        private int seed;
        private bool isSmoothed;
        public SmoothNoiseLayerVM()
        {

            Pixels = new byte[ResolutionX, ResolutionX, 4];
        }

        public bool IsSmoothed
        {
            get { return isSmoothed; }
            set
            {
                isSmoothed = value;
                OnPropertyChanged(nameof(IsSmoothed));
            }
        }

        public int Seed
        {
            get { return seed; }
            set
            {
                seed = value;
                OnPropertyChanged(nameof(Seed));
            }
        }

        public override void CalculateLayerContent()
        {
            Pixels = new byte[ResolutionX, ResolutionY, 4];
            GenerateNoise(seed.ToString());
        }

        private void GenerateNoise(string seed)
        {
            Bitmap = Perlin.Noise.GenerateWhiteNoise(ResolutionX, ResolutionY, seed);
            if (isSmoothed)
            {
                Bitmap = Perlin.Noise.GenerateSmoothNoise(Bitmap, 3);
            }

            for (int x = 0; x < ResolutionX; x++)
            {
                for (int y = 0; y < ResolutionY; y++)
                {

                    Pixels[x, y, 0] = Math.Clamp((byte)(Bitmap[x][y] * 255), (byte)0, ColorB); //Blue
                    Pixels[x, y, 1] = Math.Clamp((byte)(Bitmap[x][y] * 255), (byte)0, ColorG); //Green
                    Pixels[x, y, 2] = Math.Clamp((byte)(Bitmap[x][y] * 255), (byte)0, ColorR); //Red

                    Pixels[x, y, 3] = Opacity; //3 is alpha channel
                                               //Pixels[x, y, 0] = (byte)(bitmap[x][y] * 255); //Blue
                                               //Pixels[x, y, 1] = (byte)(bitmap[x][y] * 255); //Green
                                               //Pixels[x, y, 2] = (byte)(bitmap[x][y] * 255); //Red

                }
            }
        }
    }
}