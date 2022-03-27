using System;
using System.Collections.ObjectModel;
using System.IO;

namespace app.ViewModel {
    public class DirectoryInfoViewModel : FileSystemInfoViewModel {
        public ObservableCollection<FileSystemInfoViewModel> Items { get; private set; }
            = new ObservableCollection<FileSystemInfoViewModel>();

        public bool Open(string path) {
            try {
                foreach (var dirPath in Directory.GetDirectories(path)) {
                    var dir = new DirectoryInfoViewModel { Model = new DirectoryInfo(dirPath) };
                    dir.Open(dirPath);
                    Items.Add(dir);
                }

                foreach (var filePath in Directory.GetFiles(path)) {
                    Items.Add(new FileInfoViewModel { Model = new FileInfo(filePath) });
                }

                return true;
            } catch (Exception) { 
                return false;
            }
        }
    }
}