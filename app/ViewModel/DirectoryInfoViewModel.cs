using System;
using System.Collections.ObjectModel;
using System.IO;

namespace app.ViewModel {
    public class DirectoryInfoViewModel : FileSystemInfoViewModel {
        public ObservableCollection<FileSystemInfoViewModel> Items { get; private set; }
            = new ObservableCollection<FileSystemInfoViewModel>();

        public bool Open(string path) {
            try {
                foreach (var dirName in Directory.GetDirectories(path)) {
                    Items.Add(new DirectoryInfoViewModel { Model = new DirectoryInfo(dirName) });
                }

                foreach (var fileName in Directory.GetFiles(path)) {
                    Items.Add(new FileInfoViewModel { Model = new FileInfo(fileName) });
                }

                return true;
            } catch (Exception) { 
                return false;
            }
        }
    }
}