using MVVM.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace MVVM.Models
{
    enum BlendModes
    {
        Additive = 0,
        Multiply = 1
    }

    class LayerVM : ViewModel
    {
        private string name;
        private byte[][] bitmap;

        public byte[][] Bitmap
        {
            get { return bitmap; }
            set 
            {
                OnPropertyChanged(nameof(Bitmap));
                bitmap = value; 
            }
        }


        private BlendModes blendMode = BlendModes.Additive;

        public BlendModes BlendMode
        {
            get { return blendMode; }
            set 
            {
                OnPropertyChanged(nameof(BlendMode));
                blendMode = value; 
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
    }
}
