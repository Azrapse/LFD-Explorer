using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LfdArchiver.Applications.ViewModels;
using LfdArchiver.Applications.Views;
using LfdArchiver.Domain;

namespace LfdArchiver.Presentation.Views
{
    [Export(typeof(IShellView))]
    public partial class ShellWindow : Window, IShellView
    {
        private readonly Lazy<ShellViewModel> viewModel;
        public ShellWindow()
        {
            InitializeComponent();
            this.viewModel = new Lazy<ShellViewModel>(() => ViewHelper.GetViewModel<ShellViewModel>(this));
        }

        private bool isSelectionExtracted;
        private void lvResources_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (lvResources.SelectedItems.Count > 0 && e.LeftButton == MouseButtonState.Pressed && !isSelectionExtracted)
            {                
                var listView = (ListView)sender;                
                var selected = listView.SelectedItems
                    .Cast<ResourceEntry>()
                    .ToArray();
                var tempPath = Path.GetTempPath();
                var paths = new List<string>();                
                foreach (var entry in selected)
                {
                    var filename = string.Format("{0}.{1}", entry.Name, entry.Type);
                    var filePath = Path.Combine(tempPath, filename);
                    using (var stream = File.OpenWrite(filePath))
                    {
                        stream.Write(entry.Data, 0, entry.Data.Length);
                    }
                    paths.Add(filePath);
                }
                isSelectionExtracted = true;
                var data = new DataObject(DataFormats.FileDrop, paths.ToArray());
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            }
        }

        private void lvResources_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSelectionExtracted)
            {
                e.Handled = true;
                isSelectionExtracted = false;
            }
            
        }

        private void lvResources_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isSelectionExtracted = false;
        }

        public ResourceEntry[] GetSelectedEntries()
        {
            return lvResources.SelectedItems
                .Cast<ResourceEntry>()
                .ToArray();
        }

        private void lvResources_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {                
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                viewModel.Value.Add(files);
            }
        }
    }
}
