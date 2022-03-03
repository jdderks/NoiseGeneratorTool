using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MVVM.General
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate bool Condition();

    /// <summary>
    /// 
    /// </summary>
    public class Command : ICommand
    {
        private Condition _canExecute { get; }
        private Action _execute { get; }

        public Command(Action execute, Condition canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Command<T> : ICommand
    {
        private Condition _canExecute { get; }
        private Action<T> _execute { get; }

        public Command(Action<T> execute, Condition canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
