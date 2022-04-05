using System;
using System.Collections.Generic;
using System.Text;

namespace MVVM.Models
{
    class SmoothNoiseLayerVM : LayerVM
    {
        private int seed;

        public int Seed
        {
            get { return seed; }
            set { 
                seed = value;
                OnPropertyChanged(nameof(Seed));
            }
        }
    }
}
