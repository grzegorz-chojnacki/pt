using System;
using System.Windows.Input;

namespace app.ViewModel {
    public class FileInfoViewModel : FileSystemInfoViewModel {
        private ICommand open;
        public ICommand Open {
            get {
                return open ?? (open = new RelayCommand(param => Console.WriteLine("Open")));
            }
        }
        
        private ICommand delete;
        public ICommand Delete {
            get {
                return delete ?? (delete = new RelayCommand(param => Console.WriteLine("Delete")));
            }
        }
    }
}
