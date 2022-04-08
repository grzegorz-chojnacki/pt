using app.View;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace app.ViewModel {
    public class DirectoryInfoViewModel : FileSystemInfoViewModel {
        public ObservableCollection<FileSystemInfoViewModel> Items { get; private set; }
            = new ObservableCollection<FileSystemInfoViewModel>();

        private FileSystemWatcher Watcher;

        public bool Open(string path) {
            try {
                foreach (var dirPath in Directory.GetDirectories(path)) {
                    var dir = new DirectoryInfoViewModel { Model = new DirectoryInfo(dirPath) };
                    dir.Open(dirPath);
                    Items.Add(dir);
                }

                foreach (var filePath in Directory.GetFiles(path)) {
                    Items.Add(new FileInfoViewModel { Model = new FileInfo(filePath) });
                }

                Watcher = new FileSystemWatcher(path) { EnableRaisingEvents = true };

                Watcher.Created += OnFileSystemChange;
                Watcher.Deleted += OnFileSystemChange;
                Watcher.Changed += OnFileSystemChange;
                Watcher.Renamed += OnFileSystemRename;
                Watcher.Error += OnFileSystemError;

                return true;
            } catch (Exception) {
                return false;
            }
        }

        private void OnFileSystemChange(object sender, FileSystemEventArgs e) {
            void ThreadAction(Action f) => App.Current.Dispatcher.Invoke(delegate { f(); });

            void Create() => ThreadAction(() => Items.Add(NewFileSystemEntity(e.FullPath)));
            void Delete() => ThreadAction(() => Items.Remove(Items.Single(x => x.Name == e.Name)));

            switch (e.ChangeType) {
                case WatcherChangeTypes.Created: Create(); break;
                case WatcherChangeTypes.Deleted: Delete(); break;
                case WatcherChangeTypes.Changed: break;
            }
        }

        private void OnFileSystemRename(object sender, RenamedEventArgs e) {
           Items.Single(x => x.Name == e.OldName).Name = e.Name;
        }

        private FileSystemInfoViewModel NewFileSystemEntity(string path) {
            if (File.Exists(path)) {
                return new FileInfoViewModel { Model = new FileInfo(path) };
            } else if (Directory.Exists(path)) {
                var dir = new DirectoryInfoViewModel { Model = new DirectoryInfo(path) };
                dir.Open(path);
                return dir;
            } else {
                throw new Exception(path);
            }
        }

        private void OnFileSystemError(object sender, ErrorEventArgs e) { }

        private ICommand create;
        public ICommand Create {
            get {
                return create ?? (create = new RelayCommand(param => {
                    new CreateDialog() {
                        Title = "Create file or directory",
                        Owner = MainWindow.Instance,
                        RootPath = Model.FullName,
                    }.ShowDialog();
                }));
            }
        }
        protected override void DeleteHandler() {
            ((DirectoryInfo)Model).Delete(true);
        }
    }
}