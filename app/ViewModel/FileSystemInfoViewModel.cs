using System;
using System.IO;

namespace app.ViewModel {
    public class FileSystemInfoViewModel : ViewModelBase {
        public DateTime LastWriteTime {
            get { return LastWriteTime; }
            set {
                if (LastWriteTime != value) {
                    LastWriteTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FileSystemInfo Model {
            get { return Model; }
            set {
                if (Model != value) {
                    LastWriteTime = value.LastWriteTime;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
