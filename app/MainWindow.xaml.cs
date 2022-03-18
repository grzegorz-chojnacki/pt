﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using ControlMenuItem = System.Windows.Controls.MenuItem;

namespace app {
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
                foreach (var dirPath in Directory.GetDirectories(path)) {
                    var child = new DirectoryTreeViewItem(dirPath);
                    TraverseTree(child, dirPath);
                    node.Items.Add(child);
                };

                foreach (var filePath in Directory.GetFiles(path)) {
                    node.Items.Add(new FileTreeViewItem(filePath));
                };
            } catch (UnauthorizedAccessException) { /* Do nothing */ }

            return node;
        }

        public void SetAttributes(FileAttributes attributes) {
            MainWindow.Instance.status.Text = (((attributes & FileAttributes.ReadOnly) != 0) ? "r" : "-")
                    + (((attributes & FileAttributes.Archive) != 0) ? "a" : "-")
                    + (((attributes & FileAttributes.System) != 0) ? "s" : "-")
                    + (((attributes & FileAttributes.Hidden) != 0) ? "h" : "-");
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

        private void Exit(object sender, RoutedEventArgs e) {
            System.Windows.Forms.Application.Exit();
        }
    }
}
