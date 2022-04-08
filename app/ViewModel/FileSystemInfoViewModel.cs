using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace app.ViewModel {
    public abstract class FileSystemInfoViewModel : ViewModelBase {
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

        private ICommand delete;
        public ICommand Delete {
            get {
                return delete ?? (delete = new RelayCommand(param => {
                    void err(string msg) => System.Windows.MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                    try {
                        DeleteHandler();
                    } catch (UnauthorizedAccessException) {
                        err("Couldn't delete item, access denied.");
                    } catch (IOException) {
                        err("Couldn't delete readonly item.");
                    } catch (Exception ex) {
                        Console.WriteLine(ex);
                        err("Something happened.");
                    }
                }));
            }
        }

        protected abstract void DeleteHandler();
    }
}
