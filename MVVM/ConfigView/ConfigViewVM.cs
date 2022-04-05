using MVVM.General;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MVVM.ConfigView
{
    class ConfigViewVM : ViewModel
    {
        ConfigView _view;

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public ConfigViewVM(ConfigView view)
        {
            _view = view;
            OkCommand = new Command(OkAction);
            CancelCommand = new Command(CancelAction);
        }

        private void CancelAction()
        {
            // Sluit view
            _view.Close();
        }

        private void OkAction()
        {
            // Eventuele Ok acties


            // Sluit view
            _view.Close();
        }
    }
}

