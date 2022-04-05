using MVVM.General;
using MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVM.TaskParallelForEachView
{
    class TaskParallelForEachViewVM : ViewModel, IDataErrorInfo
    {
        #region Fields

        private Window _view;
        private ObservableCollection<LongLastingTask> _taskList;
        private string _message;
        private Task _background;
        private string _maxDegreeOfParallelism;
        private int maxDegreeOfParallelism;
        private bool _startEnabled;

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
        /// 
        /// </summary>
        public string MaxDegreeOfParallelism
        {
            get { return _maxDegreeOfParallelism; }
            set
            {
                _maxDegreeOfParallelism = value;
                OnPropertyChanged(nameof(MaxDegreeOfParallelism));
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

        /// <summary>
        /// Enabler of start button
        /// </summary>
        public bool StartEnabled
        {
            get { return _startEnabled; }
            set
            {
                _startEnabled = value;
                OnPropertyChanged(nameof(StartEnabled));
            }
        }

        #endregion

        #region Commands

        public ICommand StartCommand { get; }
        public ICommand CloseCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view">Owner view is passed as a parameter to be able to close the view from the viewmodel</param>
        public TaskParallelForEachViewVM(Window view)
        {
            _view = view;

            // Instantiate commands with the appropriate action method
            StartCommand = new Command(StartTaskParallelForEach);
            CloseCommand = new Command(CloseView);

            // Create list of 200 long-lasting tasks
            TaskList = new ObservableCollection<LongLastingTask>(GetLongLastingTasks(200));

            // Set max degree of parallelism according to number of cores
            MaxDegreeOfParallelism = Environment.ProcessorCount.ToString();

            // Enable start button
            StartEnabled = true;
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
        private void StartTaskParallelForEach()
        {
            StartEnabled = false;
            string executionTime = string.Empty;
            Message = "Background work in progress ...";

            // Reset all tasks
            foreach (var task in TaskList)
            {
                task.Reset();
            }

            // Configure background operation
            _background = new Task(() =>
            {
                DateTime startDateTime = DateTime.Now;

                var options = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                };

                Parallel.ForEach(TaskList, options, _task =>
                {
                    _task.Run();
                });

                DateTime endDateTime = DateTime.Now;
                executionTime = $"{(endDateTime - startDateTime).TotalMilliseconds:f0} ms";
                StartEnabled = true;
            });

            // Start background operation
            _background.Start();

            // Set follow-up action when background ended successfully
            _background.ContinueWith(
                (task) =>
                {
                    Message = $"Background work is done ({executionTime})";
                },
                TaskContinuationOptions.OnlyOnRanToCompletion
            );

            // Set follow-up action when background encountered an exception
            _background.ContinueWith(
                (task) =>
                {
                    Message = $"A background exception was thrown ({executionTime})";
                },
                TaskContinuationOptions.OnlyOnFaulted
            );
        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseView()
        {
            _view.Close();
        }

        #endregion

        #region IDataErrorInfo

        /// <summary>
        /// (Property is obsolete)
        /// </summary>
        public string Error => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(MaxDegreeOfParallelism):
                        if (!int.TryParse(MaxDegreeOfParallelism, out maxDegreeOfParallelism))
                        {
                            return "Not an integer";
                        }
                        if (maxDegreeOfParallelism < 1 || maxDegreeOfParallelism > 200)
                        {
                            return "MaxDegreeOfParallelism is out-of-range";
                        }
                        break;
                }

                return string.Empty;
            }
        }

        #endregion
    }
}
