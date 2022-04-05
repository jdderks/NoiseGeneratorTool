using System;
using System.Collections.Generic;
using System.Text;

namespace MVVM.Models
{
    class SubItem2VM : ItemVM
    {
        private int numberOfLayers;

        public int NumberOfLayers
        {
            get { return numberOfLayers; }
            set
            {
                numberOfLayers = value;
                OnPropertyChanged(nameof(NumberOfLayers));
            }
        }
    }
}
