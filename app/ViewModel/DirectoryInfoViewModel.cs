using app.Resources;
using app.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace app.ViewModel {
    public class DirectoryInfoViewModel : FileSystemInfoViewModel {
        public ObservableCollection<FileSystemInfoViewModel> Items { get; set; }
            = new ObservableCollection<FileSystemInfoViewModel>();

        private FileSystemWatcher Watcher;

        new public long Size { get => Items.Count(); }

        public bool isExpanded;

        public ICommand CreateCommand { get; }

        public DirectoryInfoViewModel(FileExplorer owner) : base(owner) {
            CreateCommand = new RelayCommand(param => {
                new CreateDialog() {
                    Title = Strings.CreateFileOrDirectoryPrompt,
                    Owner = owner.Window,
                    RootPath = Model.FullName,
                }.ShowDialog();
            });
        }

        public bool Open(string path) {
            try {
                foreach (var dirPath in Directory.GetDirectories(path)) {
                    var dir = new DirectoryInfoViewModel(Owner) { Model = new DirectoryInfo(dirPath) };
                    dir.Open(dirPath);
                    Items.Add(dir);
                }

                foreach (var filePath in Directory.GetFiles(path)) {
                    Items.Add(new FileInfoViewModel(Owner) { Model = new FileInfo(filePath) });
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
            void Delete() => ThreadAction(() => Items.Remove(Items.Single(x => x.Model.Name == e.Name)));

            switch (e.ChangeType) {
                case WatcherChangeTypes.Created: Create(); break;
                case WatcherChangeTypes.Deleted: Delete(); break;
                case WatcherChangeTypes.Changed: break;
            }
        }

        private void OnFileSystemRename(object sender, RenamedEventArgs e) {
            Items.Single(x => x.Model.Name == e.OldName).Model = NewFileSystemEntity(e.FullPath).Model;
        }

        private FileSystemInfoViewModel NewFileSystemEntity(string path) {
            if (File.Exists(path)) {
                return new FileInfoViewModel(Owner) { Model = new FileInfo(path) };
            } else if (Directory.Exists(path)) {
                var dir = new DirectoryInfoViewModel(Owner) { Model = new DirectoryInfo(path) };
                dir.Open(path);
                return dir;
            } else {
                throw new Exception(path);
            }
        }

        private void OnFileSystemError(object sender, ErrorEventArgs e) { }

        protected override void DeleteHandler() {
            ((DirectoryInfo)Model).Delete(true);
        }

        public void Sort(SortSettings sortSettings) {
            foreach (var item in Items) {
                if (item is DirectoryInfoViewModel) {
                    ((DirectoryInfoViewModel)item).Sort(sortSettings);
                }
            }

            var fn = (new Dictionary<SortBy, Func<FileSystemInfoViewModel, object>> {
                [SortBy.Name] = x => x.Name,
                [SortBy.Size] = x => x.Size,
                [SortBy.Extension] = x => x.Model.Extension,
                [SortBy.ModifiedDate] = x => x.LastWriteTime,
            })[sortSettings.SortBy];

            Items = new ObservableCollection<FileSystemInfoViewModel>(
                ((sortSettings.SortDirection == SortDirection.Ascending)
                    ? Items.OrderBy(fn)
                    : Items.OrderByDescending(fn))
                .OrderByDescending(item => item is DirectoryInfoViewModel));

            NotifyPropertyChanged(nameof(Items));
            NotifyPropertyChanged(nameof(isExpanded));
        }
    }
}