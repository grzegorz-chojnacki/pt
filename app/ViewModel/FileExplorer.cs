using app.Resources;
using app.View;
using System.Globalization;
using System.Windows.Forms;

namespace app.ViewModel {
    public class FileExplorer : ViewModelBase {
        public string Lang {
            get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
            set {
                if (value != null) {
                    if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != value) {
                        CultureInfo.CurrentUICulture = new CultureInfo(value);
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        public DirectoryInfoViewModel Root { get; set; }
        public RelayCommand OpenRootDirectoryCommand { get; }
        public RelayCommand SortRootDirectoryCommand { get; }
        public SortSettings SortSettings = new SortSettings();

        public MainWindow Window { get; }

        public FileExplorer(MainWindow window) {
            Window = window;

            PropertyChanged += (_, e) => {
                    if (e.PropertyName == nameof(Lang)) {
                        CultureResources.ChangeCulture(CultureInfo.CurrentUICulture);
                    }
                };

            OpenRootDirectoryCommand = new RelayCommand(_ => {
                var dialog = new FolderBrowserDialog() {
                    Description = Strings.OpenDirectoryPrompt
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    OpenDirectoryPath(dialog.SelectedPath);
                }
            });

            SortRootDirectoryCommand = new RelayCommand(_ => {
                var dialog = new SortDialog(SortSettings);
                if (dialog.ShowDialog() == true) {
                    Root.Sort(SortSettings);
                }
            }, _ => Root != null);
        }

        public void OpenDirectoryPath(string path) {
            Root = new DirectoryInfoViewModel(this);
            Root.Open(path);
            NotifyPropertyChanged(nameof(Lang));
            NotifyPropertyChanged(nameof(Root));
        }
    }
}
