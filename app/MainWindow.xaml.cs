using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace app {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private TreeViewItem createTreeViewItemNode(string path) {
            return new TreeViewItem
            {
                Header = System.IO.Path.GetFileName(path),
                Tag = path
            };
        }

        private TreeViewItem TraverseTree(TreeViewItem node, string path) {
            try {
                foreach (var dirPath in System.IO.Directory.GetDirectories(path)) {
                    var child = createTreeViewItemNode(dirPath);
                    TraverseTree(child, dirPath);
                    node.Items.Add(child);
                };

                foreach (var filePath in System.IO.Directory.GetFiles(path)) {
                    node.Items.Add(createTreeViewItemNode(filePath));
                };
            }
            catch (UnauthorizedAccessException) { /* Do nothing */ }

            return node;
        }

        private void OpenDirectory(object sender, RoutedEventArgs e) {
            var dialog = new FolderBrowserDialog() { Description = "Select directory to open" };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                treeView.Items.Clear();
                var root = TraverseTree(createTreeViewItemNode(dialog.SelectedPath), dialog.SelectedPath);
                root.IsExpanded = true;
                treeView.Items.Add(root);
            }
        }
    }
}
