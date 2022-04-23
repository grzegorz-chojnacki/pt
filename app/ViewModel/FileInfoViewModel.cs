using System.IO;
using System.Linq;
using System.Windows.Input;

namespace app.ViewModel {
    public class FileInfoViewModel : FileSystemInfoViewModel {
        public static readonly string[] TextFilesExtensions = new string[] { ".txt", ".ini", ".log" };

        public string Icon {
            get => (Model.Extension == ".txt") ? "/Resources/txt.png" : "/Resources/unknown.png";
        }

        public ICommand OpenCommand { get; }

        public FileInfoViewModel(FileExplorer owner) : base(owner) {
            OpenCommand = new RelayCommand(_ => {
                if (Model.Extension == ".txt") {
                    using (var reader = File.OpenText(Model.FullName)) {
                        owner.Window.fileView.Text = reader.ReadToEnd();
                    }
                };
            }, _ => TextFilesExtensions.Contains(Model.Extension));
        }

        protected override void DeleteHandler() => Model.Delete();
    }
}
