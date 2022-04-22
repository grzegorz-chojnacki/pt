using app.View;
using System.IO;
using System.Windows.Input;

namespace app.ViewModel {
    public class FileInfoViewModel : FileSystemInfoViewModel {

        public string Icon {
            get {
                return (Model.Extension == ".txt")
                    ? "/Resources/txt.png"
                    : "/Resources/unknown.png";
            }
        }

        private ICommand open;
        public ICommand Open {
            get {
                return open ?? (open = new RelayCommand(param => {
                    if (Model.Extension == ".txt") {
                        using (var reader = File.OpenText(Model.FullName)) {
                            MainWindow.Instance.fileView.Text = reader.ReadToEnd();
                        }
                    };
                }));
            }
        }

        protected override void DeleteHandler() {
            Model.Delete();
        }
    }
}
