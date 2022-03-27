using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using ControlMenuItem = System.Windows.Controls.MenuItem;

namespace app.Windows {
    public abstract class FileSystemTreeViewItem : TreeViewItem {
        protected string RootPath;

        protected readonly ControlMenuItem DeleteMenuItem = new ControlMenuItem() { Header = "Delete" };
        protected readonly ControlMenuItem CreateMenuItem = new ControlMenuItem() { Header = "Create" };
        protected readonly ControlMenuItem OpenMenuItem = new ControlMenuItem() { Header = "Open" };

        protected FileSystemTreeViewItem(string path) : base() {
            Header = Path.GetFileName(path);
            Tag = path;
            RootPath = path;
            ContextMenu = new System.Windows.Controls.ContextMenu();

            Selected += (sender, e) => {
                SelectedHandler();
                e.Handled = true; // Bubbling event workaround
            };

            DeleteMenuItem.Click += (sender, e) => {
                void err(string msg) => System.Windows.MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                try {
                    DeleteHandler();
                } catch (UnauthorizedAccessException) {
                    err("Couldn't delete item, access denied.");
                } catch (IOException) {
                    err("Couldn't delete readonly item.");
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    err("Something happened.");
                }
            };

            ContextMenu.Items.Add(DeleteMenuItem);
        }

        protected abstract void DeleteHandler();
        protected abstract void SelectedHandler();

        protected static TreeViewItem createTreeViewItem(string path) {
            if (File.Exists(path)) {
                return new FileTreeViewItem(path);
            } else if (Directory.Exists(path)) {
                return new DirectoryTreeViewItem(path);
            } else {
                throw new Exception(path);
            }
        }
    }

    public class FileTreeViewItem : FileSystemTreeViewItem {
        protected override void DeleteHandler() {
            File.Delete(RootPath);
            ((TreeViewItem)Parent).Items.Remove(this);
        }
        protected override void SelectedHandler() {
            MainWindow.Instance.SetAttributes(File.GetAttributes(RootPath));
        }

        public FileTreeViewItem(string path) : base(path) {
            Selected += (sender, e) => {
                e.Handled = true; // Bubbling event workaround
            };

            if (Path.GetExtension(path) == ".txt") {
                OpenMenuItem.Click += (sender, e) => {
                    using (var reader = File.OpenText(path)) {
                        MainWindow.Instance.fileView.Text = reader.ReadToEnd();
                    }
                };

                ContextMenu.Items.Insert(0, OpenMenuItem);
            }
        }
    }

    public class DirectoryTreeViewItem : FileSystemTreeViewItem {
        protected override void SelectedHandler() {
            MainWindow.Instance.SetAttributes(new DirectoryInfo(RootPath).Attributes);
        }

        protected override void DeleteHandler() {
            Directory.Delete(RootPath, true); // Delete recursively

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
                    RootPath = path,
                };

                if (dialog.ShowDialog() == true) {
                    Items.Insert(0, createTreeViewItem(dialog.FullPath));
                }
            };

            ContextMenu.Items.Insert(0, CreateMenuItem);
        }
    }
}
