using System;
using System.Windows.Input;

namespace app.ViewModel {
    public class RelayCommand : ICommand {
        Action<object> _execute;


        public RelayCommand(Action<object> execute) {
            _execute = execute;
        }

        public bool CanExecute(object parameter) {
           return true;
        }

        public event EventHandler CanExecuteChanged {
            add {
                CommandManager.RequerySuggested += value;
            }
            remove {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter) {
            _execute(parameter);
        }
    }
}