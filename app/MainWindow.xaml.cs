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
            var path = "C:\\Users\\User\\Desktop";
            var root = TraverseTree(createTreeViewItem(path), path);
            root.IsExpanded = true;
            treeView.Items.Add(root);
        }

        private TreeViewItem createTreeViewItem(string path) {
            var node = new TreeViewItem {
                Header = System.IO.Path.GetFileName(path),
                Tag = path,
            };

            var delete = new System.Windows.Controls.MenuItem() { Header = "Delete" };
            delete.Click += (sender, e) => {
                if (node.Parent is TreeViewItem) {
                    ((TreeViewItem)node.Parent).Items.Remove(node);
                } else {
                    treeView.Items.Clear();
                }

                if (System.IO.File.Exists(path)) {
                    System.IO.File.Delete(path);
                } else if (System.IO.Directory.Exists(path)) {
                    System.IO.Directory.Delete(path, true); // Delete recursively
                }
            };

            node.ContextMenu = new System.Windows.Controls.ContextMenu() { Items = { delete }};
            return node;
        }

        private TreeViewItem TraverseTree(TreeViewItem node, string path) {
            try {
                foreach (var dirPath in System.IO.Directory.GetDirectories(path)) {
                    var child = createTreeViewItem(dirPath);
                    TraverseTree(child, dirPath);
                    node.Items.Add(child);
                };

                foreach (var filePath in System.IO.Directory.GetFiles(path)) {
                    node.Items.Add(createTreeViewItem(filePath));
                };
            }
            catch (UnauthorizedAccessException) { /* Do nothing */ }

            return node;
        }

        private void OpenDirectory(object sender, RoutedEventArgs e) {
            var dialog = new FolderBrowserDialog() { Description = "Select directory to open" };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                treeView.Items.Clear();
                var root = TraverseTree(createTreeViewItem(dialog.SelectedPath), dialog.SelectedPath);
                root.IsExpanded = true;
                treeView.Items.Add(root);
            }
        }
    }
}
