using MVVM.General;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MVVM.Models
{
    /// <summary>
    /// Basic long lasting task class; other long lasting task classes can derive from this
    /// </summary>
    class LongLastingTask : ViewModel
    {
        #region Fields

        string _message;

        #endregion

        #region Properties

        /// <summary>
        /// Task number
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Status message that will be shown in the view
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

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public LongLastingTask(int id)
        {
            ID = id;
            Reset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes task
        /// </summary>
        internal virtual void Reset()
        {
            Message = $"Task {ID} is initialized";
        }

        /// <summary>
        /// Performs task, then sets message to "done"
        /// </summary>
        internal virtual void Run()
        {
            // Wait 50 ms
            Thread.Sleep(50);

            // Set message
            Message = $"Task {ID} is done";
        }

        #endregion
    }
}
