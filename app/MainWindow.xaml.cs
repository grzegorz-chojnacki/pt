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
    public abstract class FileSystemTreeViewItem : TreeViewItem {
        protected string Path;

        protected readonly System.Windows.Controls.MenuItem DeleteMenuItem = new System.Windows.Controls.MenuItem() { Header = "Delete" };
        protected readonly System.Windows.Controls.MenuItem CreateMenuItem = new System.Windows.Controls.MenuItem() { Header = "Create" };
        protected readonly System.Windows.Controls.MenuItem OpenMenuItem = new System.Windows.Controls.MenuItem() { Header = "Open" };

        protected FileSystemTreeViewItem(string path) : base() {
            Header = System.IO.Path.GetFileName(path);
            Tag = path;
            Path = path;
            ContextMenu = new System.Windows.Controls.ContextMenu();

            Selected += (sender, e) => {
                SelectedHandler();
                e.Handled = true; // Bubbling event workaround
            };

            DeleteMenuItem.Click += (sender, e) => {
                try {
                    DeleteHandler();
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    System.Windows.MessageBox.Show("Something happened", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            };

            ContextMenu.Items.Add(DeleteMenuItem);
        }

        protected abstract void DeleteHandler();
        protected abstract void SelectedHandler();

        protected static TreeViewItem createTreeViewItem(string path) {
            if (System.IO.File.Exists(path)) {
                return new FileTreeViewItem(path);
            } else if (System.IO.Directory.Exists(path)) {
                return new DirectoryTreeViewItem(path);
            } else {
                throw new Exception(path);
            }
        }
    }

    public class FileTreeViewItem : FileSystemTreeViewItem {
        protected override void DeleteHandler() {
            System.IO.File.Delete(Path);
            ((TreeViewItem)Parent).Items.Remove(this);
        }
        protected override void SelectedHandler() {
            MainWindow.Instance.status.Text = System.IO.File.GetAttributes(Path).ToString();
        }

        public FileTreeViewItem(string path) : base(path) {
            Selected += (sender, e) => {
                e.Handled = true; // Bubbling event workaround
            };

            if (System.IO.Path.GetExtension(path) == ".txt") {
                OpenMenuItem.Click += (sender, e) => {
                    using (var reader = System.IO.File.OpenText(path)) {
                        MainWindow.Instance.fileView.Text = reader.ReadToEnd();
                    }
                };

                ContextMenu.Items.Insert(0, OpenMenuItem);
            }
        }
    }

    public class DirectoryTreeViewItem : FileSystemTreeViewItem {
        protected override void SelectedHandler() {
            MainWindow.Instance.status.Text = new System.IO.DirectoryInfo(Path).Attributes.ToString();
        }

        protected override void DeleteHandler() {
            System.IO.Directory.Delete(Path, true); // Delete recursively

            if (Parent is TreeViewItem item) {
                item.Items.Remove(this);
            } else {
                MainWindow.Instance.treeView.Items.Clear();
            }
        }
        public DirectoryTreeViewItem(string path) : base(path) {
            CreateMenuItem.Click += (sender, e) => {
                var dialog = new CreateDialog() {
                    Title = "Create file or directory",
                    Owner = MainWindow.Instance,
                    Path = path,
                };

                if (dialog.ShowDialog() == true) {
                    Items.Insert(0, createTreeViewItem(dialog.fullPath));
                }
            };

            ContextMenu.Items.Insert(0, CreateMenuItem);
        }
    }

    public partial class MainWindow : Window {
        public static MainWindow Instance;
        public MainWindow() {
            InitializeComponent();
            Instance = this;

            var path = "C:\\Users\\User\\Desktop";
            var root = TraverseTree(new DirectoryTreeViewItem(path), path);
            root.IsExpanded = true;
            treeView.Items.Add(root);
        }

        private TreeViewItem TraverseTree(TreeViewItem node, string path) {
            try {
                foreach (var dirPath in System.IO.Directory.GetDirectories(path)) {
                    var child = new DirectoryTreeViewItem(dirPath);
                    TraverseTree(child, dirPath);
                    node.Items.Add(child);
                };

                foreach (var filePath in System.IO.Directory.GetFiles(path)) {
                    node.Items.Add(new FileTreeViewItem(filePath));
                };
            } catch (UnauthorizedAccessException) { /* Do nothing */ }

            return node;
        }

        private void OpenDirectory(object sender, RoutedEventArgs e) {
            var dialog = new FolderBrowserDialog() { Description = "Select directory to open" };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                treeView.Items.Clear();
                var root = TraverseTree(new DirectoryTreeViewItem(dialog.SelectedPath), dialog.SelectedPath);
                root.IsExpanded = true;
                treeView.Items.Add(root);
            }
        }
    }
}
