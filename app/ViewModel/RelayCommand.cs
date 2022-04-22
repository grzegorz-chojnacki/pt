using System;
using System.Windows.Input;

namespace app.ViewModel {
    public class RelayCommand : ICommand {
        Action<object> _execute;
        Predicate<object> _canExecute = _ => true;

        public RelayCommand(Action<object> execute) {
            _execute = execute;
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) {
           return _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter) {
            _execute(parameter);
        }
    }
}