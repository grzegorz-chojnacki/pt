using app.Resources;
using app.View;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace app.ViewModel {
    public class FileExplorer : ViewModelBase {
        public string Lang {
            get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
            set {
                if (value != null) {
                    if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != value) {
                        CultureInfo.CurrentUICulture = new CultureInfo(value);
                        NotifyPropertyChanged(nameof(Lang));
                    }
                }
            }
        }

        private enum Status { Custom, Ready, Cancelled };

        private Status StatusMessageState = Status.Custom;
        private string statusMessage = "";
        public string StatusMessage {
            get {
                switch (StatusMessageState) {
                    case Status.Ready: return Strings.ReadyStatus;
                    case Status.Cancelled: return Strings.CancelledStatus;
                    default: return statusMessage;
                }
            }
            set {
                if (statusMessage != value) {
                    StatusMessageState = Status.Custom;
                    statusMessage = value;
                    NotifyPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        public int MaxThreadId = 0;
        public int ThreadCount = 0;

        public DirectoryInfoViewModel Root { get; set; }
        public RelayCommand OpenRootDirectoryCommand { get; }
        public RelayCommand SortRootDirectoryCommand { get; }
        public SortSettings SortSettings = new SortSettings();
        public RelayCommand CancelSortingCommand { get; }
        public BooleanToVisibilityConverter BooleanToVisibility = new BooleanToVisibilityConverter();

        private CancellationTokenSource CancelTokenSource;


        public MainWindow Window { get; }

        public FileExplorer(MainWindow window) {
            Window = window;

            PropertyChanged += (_, e) => {
                if (e.PropertyName == nameof(Lang)) {
                    CultureResources.ChangeCulture(CultureInfo.CurrentUICulture);
                    NotifyPropertyChanged(nameof(StatusMessage));
                }
            };

            OpenRootDirectoryCommand = new RelayCommand(async _ => {
                var dialog = new FolderBrowserDialog() {
                    Description = Strings.OpenDirectoryPrompt
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    await Task.Factory.StartNew(() => {
                        OpenDirectoryPath(dialog.SelectedPath);
                    });
                }
            });

            SortRootDirectoryCommand = new RelayCommand(async _ => {
                var dialog = new SortDialog(SortSettings);
                if (dialog.ShowDialog() == true) {
                    MaxThreadId = ThreadCount = 0;

                    CancelTokenSource = new CancellationTokenSource();
                    try {
                        await Task.Factory.StartNew(async x => {
                                await Root.Sort(SortSettings, CancelTokenSource.Token);
                            },
                            CancelTokenSource.Token,
                            TaskCreationOptions.LongRunning
                            | TaskCreationOptions.PreferFairness
                        ).Unwrap();

                        StatusMessageState = Status.Ready;
                    } catch (OperationCanceledException) {
                        Debug.WriteLine("Task cancelled");
                        StatusMessageState = Status.Cancelled;
                    } finally {
                        Debug.WriteLine($"MaxThreadId: {MaxThreadId}");
                        Debug.WriteLine($"ThreadCount: {ThreadCount}");
                        Debug.WriteLine("---------------------------");

                        CancelTokenSource.Dispose();
                        CancelTokenSource = null;
                    }
                }
            }, _ => Root != null);

            CancelSortingCommand = new RelayCommand(_ => {
                CancelTokenSource.Cancel();
            }, _ => (CancelTokenSource != null)
                 && (CancelTokenSource.Token.CanBeCanceled));
        }

        public void OpenDirectoryPath(string path) {
            Root = new DirectoryInfoViewModel(this) { Model = new DirectoryInfo(path) };
            Root.PropertyChanged += (object sender, PropertyChangedEventArgs args) => {
                if (args.PropertyName == "StatusMessage" && sender is FileSystemInfoViewModel viewModel) {
                    StatusMessage = viewModel.StatusMessage;
                }
            };

            NotifyPropertyChanged(nameof(Root));
            NotifyPropertyChanged(nameof(Lang));
            Root.Open(path);
            StatusMessageState = Status.Ready;
        }
    }
}
