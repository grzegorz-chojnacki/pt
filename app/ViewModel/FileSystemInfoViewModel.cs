using System;
using System.IO;

namespace app.ViewModel {
    public class FileSystemInfoViewModel : ViewModelBase {
        private DateTime lastWriteTime;
        public DateTime LastWriteTime {
            get { return lastWriteTime; }
            set {
                if (lastWriteTime != value) {
                    lastWriteTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string name;
        public string Name {
            get { return name; }
            set {
                if (name != value) {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private FileSystemInfo model;
        public FileSystemInfo Model {
            get { return model; }
            set {
                if (model != value) {
                    model = value;
                    LastWriteTime = value.LastWriteTime;
                    Name = value.Name;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
