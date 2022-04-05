using System;
using System.Collections.Generic;
using System.Text;

namespace MVVM.Models
{
    class SubItem1VM : ItemVM
    {
        private int opacity;

        public int Opacity
        {
            get { return opacity; }
            set
            {
                opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }
    }
}
