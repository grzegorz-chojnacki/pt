using app.Resources;
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
                    NotifyPropertyChanged(nameof(LastWriteTime));
                }
            }
        }

        private bool isExpanded = false;
        public bool IsExpanded {
            get { return isExpanded; }
            set {
                if (isExpanded != value) {
                    isExpanded = value;
                    NotifyPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        private string name;
        public string Name {
            get { return name; }
            set {
                if (name != value) {
                    name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        private long size;
        public long Size {
            get { return size; }
            set {
                if (size != value) {
                    size = value;
                    NotifyPropertyChanged(nameof(Size));
                }
            }
        }

        private String statusMessage;
        public String StatusMessage {
            get { return statusMessage; }
            set {
                if (statusMessage != value) {
                    statusMessage = value;
                    NotifyPropertyChanged(nameof(StatusMessage));
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

                    try {
                        Size = new FileInfo(value.FullName).Length;
                    } catch (Exception) {
                        Size = 0;
                    }

                    Name = value.Name;
                    NotifyPropertyChanged();
                }
            }
        }

        public FileExplorer Owner { get; }

        public ICommand DeleteCommand { get; }

        public FileSystemInfoViewModel(FileExplorer owner) : base() {
            Owner = owner;
            DeleteCommand = new RelayCommand(param => {
                void err(string msg) => System.Windows.MessageBox.Show(msg,
                    Strings.Error,
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                try {
                    DeleteHandler();
                } catch (UnauthorizedAccessException) {
                    err(Strings.UnauthorizedIOError);
                } catch (IOException) {
                    err(Strings.GeneralIOError);
                } catch (Exception) {
                    err(Strings.OtherIOError);
                }
            });
        }


        protected abstract void DeleteHandler();
    }
}
