using System.Globalization;

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

        public FileExplorer() {
            NotifyPropertyChanged(nameof(Lang));
        }

        public void OpenRoot(string path) {
            Root = new DirectoryInfoViewModel();
            Root.Open(path);
        }
    }
}
