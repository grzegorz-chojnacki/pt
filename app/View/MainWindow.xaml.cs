﻿using app.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace app.View {
    public partial class MainWindow : Window {
        public static MainWindow Instance;

        public MainWindow() {
            InitializeComponent();
            Instance = this;
            var fileExplorer = new FileExplorer();
            fileExplorer.OpenDirectoryPath("C:\\Users\\User\\Downloads");
            DataContext = fileExplorer;
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
            Instance.status.Text = (((attributes & FileAttributes.ReadOnly) != 0) ? "r" : "-")
                    + (((attributes & FileAttributes.Archive) != 0) ? "a" : "-")
                    + (((attributes & FileAttributes.System) != 0) ? "s" : "-")
                    + (((attributes & FileAttributes.Hidden) != 0) ? "h" : "-");
        }

        private void Exit(object sender, RoutedEventArgs e) {
            System.Windows.Forms.Application.Exit();
        }
    }
}
