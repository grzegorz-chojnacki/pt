using app.Resources;
using System.ComponentModel;
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

        public FileExplorer(string path) {
            Root = new DirectoryInfoViewModel();
            PropertyChanged += fileExplorerPropertyChanged;
            Root.Open(path);
            NotifyPropertyChanged(nameof(Lang));
        }

        public void fileExplorerPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Lang))
                CultureResources.ChangeCulture(CultureInfo.CurrentUICulture);
        }
    }
}
