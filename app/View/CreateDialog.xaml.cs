using System;
using System.Windows;
using System.IO;
using app.Resources;

namespace app.View {
    public partial class CreateDialog : Window {
        public string RootPath;
        public string FullPath;

        public CreateDialog() {
            InitializeComponent();
        }

        private void OkButton(object sender, RoutedEventArgs e) {
            void err(string msg) => MessageBox.Show(msg,
                Strings.Error,
                MessageBoxButton.OK, MessageBoxImage.Warning);

            try {
                FullPath = Path.Combine(RootPath, name.Text);
                if (name.Text == "" || File.Exists(FullPath) || Directory.Exists(FullPath)) throw new ArgumentException();

                var attributes = (attributeArchive.IsChecked == true ? FileAttributes.Archive : 0)
                    | (attributeHidden.IsChecked == true ? FileAttributes.Hidden : 0)
                    | (attributeReadOnly.IsChecked == true ? FileAttributes.ReadOnly : 0)
                    | (attributeSystem.IsChecked == true ? FileAttributes.System : 0);

                if (typeFile.IsChecked == true) {
                    File.Create(FullPath);
                } else if (typeDirectory.IsChecked == true) {
                    Directory.CreateDirectory(FullPath);
                }

                File.SetAttributes(FullPath, attributes);
                DialogResult = true;
            } catch (ArgumentException) {
                err(Strings.InvalidNameIOError);
            } catch (Exception ex) {
                Console.WriteLine(ex);
                err(Strings.OtherIOError);
            }
        }
    }
}
