using System.Windows;
using app.ViewModel;

namespace app.View {
    public partial class SortDialog : Window {
        public SortSettings Settings { get; }

        public SortDialog(SortSettings settings) {
            InitializeComponent();
            DataContext = settings;
            Settings = settings;
        }

        private void OkButton(object sender, RoutedEventArgs e) {
            System.Console.WriteLine(Settings.SortBy);
            System.Console.WriteLine(Settings.SortDirection);
        }
    }
}
