using app.Resources;
using app.View;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace app.ViewModel {
    public class DirectoryInfoViewModel : FileSystemInfoViewModel {
        public DispatchedObservableCollection<FileSystemInfoViewModel> Items { get; set; }
            = new DispatchedObservableCollection<FileSystemInfoViewModel>();

        private FileSystemWatcher Watcher;

        new public long Size { get => Items.Count(); }

        public ICommand CreateCommand { get; }

        public DirectoryInfoViewModel(FileExplorer owner) : base(owner) {
            CreateCommand = new RelayCommand(param => {
                new CreateDialog() {
                    Title = Strings.CreateFileOrDirectoryPrompt,
                    Owner = owner.Window,
                    RootPath = Model.FullName,
                }.ShowDialog();
            });

            void ItemsPropertyChanged(object sender, PropertyChangedEventArgs args) {
                if (args.PropertyName == "StatusMessage" && sender is FileSystemInfoViewModel viewModel) {
                    StatusMessage = viewModel.StatusMessage;
                }
            };

            Items.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs args) => {
                if (args.NewItems == null) return;

                switch (args.Action) {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in args.NewItems.Cast<FileSystemInfoViewModel>()) {
                            item.PropertyChanged += ItemsPropertyChanged;
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in args.NewItems.Cast<FileSystemInfoViewModel>()) {
                            item.PropertyChanged -= ItemsPropertyChanged;
                        }
                        break;
                }
            };
        }

        public bool Open(string path) {
            try {
                StatusMessage = $"{Strings.OpenStatus} {Model.Name}...";

                foreach (var dirPath in Directory.GetDirectories(path)) {
                    var dir = new DirectoryInfoViewModel(Owner) { Model = new DirectoryInfo(dirPath) };
                    Items.Add(dir);
                    dir.Open(dirPath);
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

            void Delete() => ThreadAction(() => {
                if (!Items.Any(x => x.Model.Name == e.Name)) return;
                Items.Remove(Items.First(x => x.Model.Name == e.Name));
            });

            void Create() => ThreadAction(() => {
                var entity = NewFileSystemEntity(e.FullPath);
                if (entity != null) {
                    Items.Add(entity);
                }
            });

            switch (e.ChangeType) {
                case WatcherChangeTypes.Created: Create(); break;
                case WatcherChangeTypes.Deleted: Delete(); break;
                case WatcherChangeTypes.Changed: break;
            }
        }

        private void OnFileSystemRename(object sender, RenamedEventArgs e) {
            if (!Items.Any(x => x.Model.Name == e.OldName)) return;

            var entity = NewFileSystemEntity(e.FullPath);
            var target = Items.First(x => x.Model.Name == e.OldName);
            if (entity != null) {
                target.Model = entity.Model;
            } else {
                Items.Remove(target);
            }
        }

        private FileSystemInfoViewModel NewFileSystemEntity(string path) {
            if (File.Exists(path)) {
                return new FileInfoViewModel(Owner) { Model = new FileInfo(path) };
            } else if (Directory.Exists(path)) {
                var dir = new DirectoryInfoViewModel(Owner) { Model = new DirectoryInfo(path) };
                dir.Open(path);
                return dir;
            } else {
                return null;
            }
        }

        private void OnFileSystemError(object sender, ErrorEventArgs e) { }

        protected override void DeleteHandler() => ((DirectoryInfo)Model).Delete(true);

        public async Task Sort(SortSettings sortSettings, CancellationToken Token) {
            void CheckIfCancelled() {
                if (Token.IsCancellationRequested) {
                    Debug.WriteLine("CancellationRequested");
                    Token.ThrowIfCancellationRequested();
                }
            }

            var tasks = new List<Task>();
            foreach (var item in Items) {
                CheckIfCancelled();
                if (item is DirectoryInfoViewModel dir) {
                    tasks.Add(Task.Factory.StartNew(async () => {
                        Debug.WriteLine($"Sorting: {dir.Name}");

                        Owner.ThreadCount++;
                        if (Owner.MaxThreadId < Thread.CurrentThread.ManagedThreadId) {
                            Owner.MaxThreadId = Thread.CurrentThread.ManagedThreadId;
                        }

                        await dir.Sort(sortSettings, Token);
                    }, TaskCreationOptions.LongRunning
                     | TaskCreationOptions.PreferFairness).Unwrap());
                }
            }

            var fn = (new Dictionary<SortBy, Func<FileSystemInfoViewModel, object>> {
                [SortBy.Name] = x => x.Name,
                [SortBy.Size] = x => x.Size,
                [SortBy.Extension] = x => x.Model.Extension,
                [SortBy.ModifiedDate] = x => x.LastWriteTime,
            })[sortSettings.SortBy];

            CheckIfCancelled();
            StatusMessage = $"{Strings.SortStatus} {Model.Name}...";
            Items = new DispatchedObservableCollection<FileSystemInfoViewModel>(
                ((sortSettings.SortDirection == SortDirection.Ascending)
                    ? Items.OrderBy(fn)
                    : Items.OrderByDescending(fn))
                .OrderByDescending(item => item is DirectoryInfoViewModel));

            await Task.WhenAll(tasks);
            NotifyPropertyChanged(nameof(Items));
        }
    }
}