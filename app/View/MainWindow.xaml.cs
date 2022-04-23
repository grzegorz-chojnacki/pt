using app.ViewModel;
using System.IO;
using System.Windows;

namespace app.View {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var fileExplorer = new FileExplorer(this);
            fileExplorer.OpenDirectoryPath("C:\\Users\\User\\Downloads");
            DataContext = fileExplorer;
        }

        public void SetAttributes(FileAttributes attributes) {
            status.Text = (((attributes & FileAttributes.ReadOnly) != 0) ? "r" : "-")
                    + (((attributes & FileAttributes.Archive) != 0) ? "a" : "-")
                    + (((attributes & FileAttributes.System) != 0) ? "s" : "-")
                    + (((attributes & FileAttributes.Hidden) != 0) ? "h" : "-");
        }

        private void Exit(object sender, RoutedEventArgs e) {
            Close();
            return;
        }
    }
}
