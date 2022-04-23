using System.IO;
using System.Windows.Input;

namespace app.ViewModel {
    public class FileInfoViewModel : FileSystemInfoViewModel {

        public string Icon {
            get => (Model.Extension == ".txt") ? "/Resources/txt.png" : "/Resources/unknown.png";
        }

        public ICommand OpenCommand { get; }

        public FileInfoViewModel(FileExplorer owner) : base(owner) {
            OpenCommand = new RelayCommand(param => {
                if (Model.Extension == ".txt") {
                    using (var reader = File.OpenText(Model.FullName)) {
                        owner.Window.fileView.Text = reader.ReadToEnd();
                    }
                };
            });
        }

        protected override void DeleteHandler() => Model.Delete();
    }
}
