using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace app {
    public partial class CreateDialog : Window {
        public string Path;
        public string fullPath;

        public CreateDialog() {
            InitializeComponent();
        }

        private void OkButton(object sender, RoutedEventArgs e) {
            try {
                fullPath = System.IO.Path.Combine(Path, name.Text);
                var attributes = (attributeArchive.IsChecked == true ? System.IO.FileAttributes.Archive : 0)
                    | (attributeHidden.IsChecked == true ? System.IO.FileAttributes.Hidden : 0)
                    | (attributeReadOnly.IsChecked == true ? System.IO.FileAttributes.ReadOnly : 0)
                    | (attributeSystem.IsChecked == true ? System.IO.FileAttributes.System : 0);

                if (typeFile.IsChecked == true) {
                    System.IO.File.Create(fullPath);
                } else if (typeDirectory.IsChecked == true) {
                    System.IO.Directory.CreateDirectory(fullPath);
                }

                System.IO.File.SetAttributes(fullPath, attributes);
                DialogResult = true;
            } catch (Exception ex) {
                Console.WriteLine(ex);
                MessageBox.Show("Something happened", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
