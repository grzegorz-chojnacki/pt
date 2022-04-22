using app.Resources;
using app.View;
using System.ComponentModel;
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

        public FileExplorer() {
            PropertyChanged += fileExplorerPropertyChanged;
            OpenRootDirectoryCommand = new RelayCommand(_ => {
                var dialog = new FolderBrowserDialog() {
                    Description = Strings.OpenDirectoryPrompt
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    OpenDirectoryPath(dialog.SelectedPath);
                }
            });
            SortRootDirectoryCommand = new RelayCommand(_ => {
                var dialog = new SortDialog();
                dialog.ShowDialog();
            }, _ => Root != null);
        }

        public DirectoryInfoViewModel Root { get; set; }
        public RelayCommand OpenRootDirectoryCommand { get; private set; }
        public RelayCommand SortRootDirectoryCommand { get; private set; }

        public void OpenDirectoryPath(string path) {
            Root = new DirectoryInfoViewModel();
            Root.Open(path);
            NotifyPropertyChanged(nameof(Lang));
            NotifyPropertyChanged(nameof(Root));
        }

        public void fileExplorerPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Lang)) {
                CultureResources.ChangeCulture(CultureInfo.CurrentUICulture);
            }
        }
    }
}
