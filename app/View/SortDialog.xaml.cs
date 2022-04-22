using System;
using System.Windows;
using System.IO;
using app.Resources;

namespace app.View {
    public enum SortBy { Name, Extension, Size, ModifiedDate }
    public enum SortDirection { Ascending, Descending }

    public partial class SortDialog : Window {

        public SortDialog() {
            InitializeComponent();
        }

        private void OkButton(object sender, RoutedEventArgs e) {
            void err(string msg) => MessageBox.Show(msg,
                Strings.Error,
                MessageBoxButton.OK, MessageBoxImage.Warning);

            try {
                DialogResult = true;
            } catch (Exception) { }
        }
    }
}
