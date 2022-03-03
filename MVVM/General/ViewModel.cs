using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MVVM.General
{
    /// <summary>
    /// View model base class
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
