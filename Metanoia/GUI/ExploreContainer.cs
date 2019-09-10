﻿using Metanoia.Formats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Metanoia.GUI
{
    public partial class ExploreContainer : DockContent
    {
        private class FolderNode : TreeNode
        {

        }

        private class FileNode : TreeNode
        {

        }

        private IContainerFormat File { get; set; }

        private ContextMenu FolderContextMenu = new ContextMenu();
        private ContextMenu FileContextMenu = new ContextMenu();

        public ExploreContainer(IContainerFormat container)
        {
            InitializeComponent();
            File = container;
            
            FolderNode folder = new FolderNode();
            folder.Text = container.GetType().Name;
            fileTree.Nodes.Add(folder);

            foreach (var file in File.GetFiles())
            {
                AddFilePath(file, folder);
            }

            {
                MenuItem item = new MenuItem("Export");
                item.Click += (sender, args) =>
                {
                    if(fileTree.SelectedNode is FileNode file)
                    {
                        var info = file.Tag as FileItem;
                        FileTools.SaveFile(info);
                    }
                };
                FileContextMenu.MenuItems.Add(item);
            }
        }
        
        private void AddFilePath(FileItem item, FolderNode folder)
        {
            var paths = item.FilePath.Split(new char[] { '/', '\\'});

            Console.WriteLine(paths.Length);

            if(paths.Length == 1)
            {
                folder.Nodes.Add(new FileNode() { Text = paths[0], Tag = item });
                return;
            }

            for (int i = 0; i < paths.Length - 1; i++)
            {
                bool found = false;
                foreach (var v in folder.Nodes)
                {
                    if (v is FolderNode fod && fod.Text == paths[i])
                    {
                        folder = fod;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    var newf = new FolderNode() { Text = paths[i] };
                    folder.Nodes.Add(newf);
                    folder = newf;
                }
            }
            folder.Nodes.Add(new FileNode() { Text = paths[paths.Length - 1], Tag = item });
        }

        private void fileTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(e.Node is FileNode f && e.Node.Tag is FileItem item)
            {
                propertyGrid1.SelectedObject = item;
            }
        }

        private void fileTree_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                var node = fileTree.GetNodeAt(e.Location);

                if (node is FileNode file)
                {
                    FileContextMenu.Show(fileTree, e.Location);
                }
                if (node is FolderNode folder)
                {
                    FolderContextMenu.Show(fileTree, e.Location);
                }
            }

        }
    }
}
