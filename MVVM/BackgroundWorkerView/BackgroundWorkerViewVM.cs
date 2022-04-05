using MVVM.General;
using MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MVVM.BackgroundWorkerView
{
    /// <summary>
    /// Example of general view with background worker
    /// </summary>
    class BackgroundWorkerViewVM : ViewModel
    {
        #region Fields

        private Window _view;
        private ObservableCollection<LongLastingTask> _taskList;
        private int _progress;
        private string _message;
        private BackgroundWorker _backgroundWorker;
        private int index;
        private DateTime startDateTime;

        #endregion

        #region Properties

        /// <summary>
        /// List (as an ObservableCollection) with tasks taking a long time to perform
        /// </summary>
        public ObservableCollection<LongLastingTask> TaskList
        {
            get { return _taskList; }
            set
            {
                _taskList = value;
                OnPropertyChanged(nameof(TaskList));
            }
        }

        /// <summary>
        /// Progress status shown in progress bar
        /// </summary>
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        /// <summary>
        /// Status message to user
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        #endregion

        #region Commands

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand CloseCommand { get; }

        #endregion

        #region Constructor with helper methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view">Owner view is passed as a parameter to be able to close the view from the viewmodel</param>
        public BackgroundWorkerViewVM(Window view)
        {
            _view = view;

            // Instantiate commands with the appropriate action method, and - optionally - the condition that will enable/disable the command
            StartCommand = new Command(StartBackgroundWorker, () => !_backgroundWorker.IsBusy);
            StopCommand = new Command(StopBackgroundWorker, () => _backgroundWorker.IsBusy);
            CloseCommand = new Command(CloseView);

            // Set BackgroundWorker
            _backgroundWorker = GetBackgroundWorker();

            // Create list of 200 long-lasting tasks
            TaskList = new ObservableCollection<LongLastingTask>(GetLongLastingTasks(200));

            // Set index at first item in TaskList; statement is not strictly necessary
            index = 0;

        }

        /// <summary>
        /// Returns BackgroundWorker wired-up for performing long lasting tasks in the background
        /// </summary>
        /// <returns></returns>
        private BackgroundWorker GetBackgroundWorker()
        {
            // Instantiate BackgroundWorker such that it reports progress and can be cancelled by the user
            var backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            // Attach event handler with actual background work (will run in background on separate thread)
            backgroundWorker.DoWork += backgroundWorker_DoWork;

            // Attach event handler that will allow the BackgroundWorker to report progress (will run in foreground)
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

            // Attach event handler that will run when BackgroundWorker has completed (will run in foreground)
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            // Return backgroundworker
            return backgroundWorker;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private IEnumerable<LongLastingTask> GetLongLastingTasks(int number)
        {
            return Enumerable.Range(1, number).Select(i => new LongLastingTask(i));
        }

        #endregion

        #region Action methods

        /// <summary>
        /// 
        /// </summary>
        private void StartBackgroundWorker()
        {
            // When all tasks are already done, prepare TaskList and index for a restart
            if (index == TaskList.Count)
            {
                // Reset all tasks
                foreach(var task in TaskList)
                {
                    task.Reset();
                }

                // Set index to 0
                index = 0;
            }

            // Start background worker
            startDateTime = DateTime.Now;
            _backgroundWorker.RunWorkerAsync();
            Message = "Background work in progress ...";
        }

        /// <summary>
        /// 
        /// </summary>
        private void StopBackgroundWorker()
        {
            // Request background worker to stop
            _backgroundWorker.CancelAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseView()
        {
            _view.Close();
        }

        #endregion

        #region Backgroundworker event handlers

        /// <summary>
        /// Here comes the actual background work that will run on the background thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (index < TaskList.Count)
            {
                if (_backgroundWorker.CancellationPending)
                {
                    // Set e.Cancel to true to notify the UI thread that background worker was cancelled by user
                    e.Cancel = true;
                    return;
                }

                // Perform the long lasting task at hand, increase index by one
                TaskList[index++].Run();

                // Report progress
                int progress = (int)(index / (TaskList.Count * 0.01));
                _backgroundWorker.ReportProgress(progress);
            }
        }

        /// <summary>
        /// Event handler to update UI with progress info from _backgroundWorker_DoWork; will run in foreground (= UI thread)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        /// <summary>
        /// Event handler that will the UI when background work is complete; will run in foreground (= UI thread)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DateTime endDateTime = DateTime.Now;
            string executionTime = $"{(endDateTime - startDateTime).TotalMilliseconds:f0} ms";

            // Show message depending how background work was ended
            if (e.Error != null)
            {
                // Report error when an exception was thrown
                Message = $"A background exception was thrown ({e.Error.Message})";
            }
            else if (e.Cancelled)
            {
                // Report when background work was cancelled by user
                Message = $"Background work was cancelled by user ({executionTime})";
            }
            else
            {
                // Report when background tasks were all completed normally
                Message = $"Background work is done ({executionTime})";
            }

            // Trick to set buttons in correct position after completion
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion
    }
}
