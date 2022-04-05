using MVVM.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace MVVM.Models
{
    class ItemVM : ViewModel
    {
        private string name;

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
