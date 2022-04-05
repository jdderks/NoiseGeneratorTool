using MVVM.General;
using MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MVVM.MainView
{
    class MainWindowVM : ViewModel
    {
        #region Fields

        private string text;
        private ObservableCollection<ItemVM> items;
        private ItemVM selectedItem;

        #endregion

        #region Properties

        public ObservableCollection<ItemVM> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public ItemVM SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        #endregion

        #region Commands

        public ICommand ExitCommand { get; }
        public ICommand ConfigCommand { get; }
        public ICommand StartBackgroundWorkerCommand { get; }
        public ICommand StartTaskParallelForEachCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Main constructor
        /// </summary>
        public MainWindowVM()
        {
            ExitCommand = new Command(ExitAction);
            ConfigCommand = new Command(ConfigAction);
            StartBackgroundWorkerCommand = new Command(OpenBackgroundWorkerView);
            StartTaskParallelForEachCommand = new Command(OpenTaskParallelForEachView);

            Items = new ObservableCollection<ItemVM>();
            Items.Add(new ItemVM() { Name = "Item 1" });
            Items.Add(new SubItem1VM() { Name = "Item 2", Opacity = 50 });
            Items.Add(new ItemVM() { Name = "Item 3" });
            Items.Add(new ItemVM() { Name = "Item 4" });
            Items.Add(new SubItem2VM() { Name = "Item 5", NumberOfLayers = 10 });
            Items.Add(new ItemVM() { Name = "Item 6" });
            Items.Add(new ItemVM() { Name = "Item 7" });
            Items.Add(new SubItem2VM() { Name = "Item 8", NumberOfLayers = 20 });

        }

        #endregion

        #region Action methods

        /// <summary>
        /// 
        /// </summary>
        private void OpenBackgroundWorkerView()
        {
            // Instantiate view
            var view = new BackgroundWorkerView.BackgroundWorkerView();
            // Assign viewmodel
            view.DataContext = new BackgroundWorkerView.BackgroundWorkerViewVM(view);
            // Open view non-modal
            view.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenTaskParallelForEachView()
        {
            // Instantiate view
            var view = new TaskParallelForEachView.TaskParallelForEachView();
            // Assign viewmodel
            view.DataContext = new TaskParallelForEachView.TaskParallelForEachViewVM(view);
            // Open view non-modal
            view.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConfigAction()
        {
            var configView = new ConfigView.ConfigView();
            configView.DataContext = new ConfigView.ConfigViewVM(configView);
            //configView.Show();
            var result = configView.ShowDialog();
            //if (result == true)
            //{
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExitAction()
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
