using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Input;
using LfdArchiver.Applications.Views;
using LfdArchiver.Domain;
using Microsoft.Win32;

namespace LfdArchiver.Applications.ViewModels
{
    [Export]
    internal class ShellViewModel : ViewModel<IShellView>
    {
        private readonly DelegateCommand exitCommand;
        private readonly DelegateCommand openCommand;
        private readonly DelegateCommand saveCommand;
        private readonly DelegateCommand saveAsCommand;
        private readonly DelegateCommand extractCommand;
        private readonly DelegateCommand addCommand;
        private readonly DelegateCommand deleteCommand;

        [ImportingConstructor]
        public ShellViewModel(IShellView view)
            : base(view)
        {
            exitCommand = new DelegateCommand(Close);
            openCommand = new DelegateCommand(Open);
            saveCommand = new DelegateCommand(Save);
            saveAsCommand = new DelegateCommand(SaveAs);
            extractCommand = new DelegateCommand(Extract);
            addCommand = new DelegateCommand(Add);
            deleteCommand = new DelegateCommand(Delete);
        }

        public ResourceArchive Archive { get; set; }

        public string Title { get { return string.IsNullOrWhiteSpace(Archive.Path) ? ApplicationInfo.ProductName : Archive.Path; } }

        public ICommand ExitCommand { get { return exitCommand; } }
        public ICommand OpenCommand { get { return openCommand; } }
        public ICommand SaveCommand { get { return saveCommand; } }
        public ICommand SaveAsCommand { get { return saveAsCommand; } }
        public ICommand ExtractCommand { get { return extractCommand; } }
        public ICommand AddCommand { get { return addCommand; } }
        public ICommand DeleteCommand { get { return deleteCommand; } }

        public Visibility ShowNoTableOfContentsWarning
        {
            get
            {
                return !Archive.HasTableOfContents
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public void Show()
        {
            ViewCore.Show();
        }

        private void Close()
        {
            ViewCore.Close();
        }

        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "LucasFilm Data archive (.LFD)|*.LFD";
            if (openFileDialog.ShowDialog() == true)
            {
                Archive.Load(openFileDialog.FileName);
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(ShowNoTableOfContentsWarning));
            }
        }
        
        private void Save()
        {
            Archive.Save();
        }

        private void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = string.IsNullOrWhiteSpace(Archive.Path) 
                ? "UNNAMED.LFD" 
                : Archive.Path;
            saveFileDialog.Filter = "LucasFilm Data archive (.LFD)|*.LFD";
            if(saveFileDialog.ShowDialog() == true)
            {
                Archive.Save(saveFileDialog.FileName);
                RaisePropertyChanged(nameof(Title));
            }
        }

        private void Extract()
        {
            var selectedEntries = ViewCore.GetSelectedEntries();
            if (!selectedEntries.Any())
            {
                return;
            }
            var firstName = string.Format("{0}.{1}", selectedEntries[0].Name, selectedEntries[0].Type);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = firstName;            
            if (saveFileDialog.ShowDialog() == true)
            {
                var selectedFolder = Path.GetDirectoryName(saveFileDialog.FileName);
                foreach(var entry in selectedEntries)
                {
                    var filename = string.Format("{0}.{1}", entry.Name, entry.Type);
                    if (filename == firstName)
                    {
                        filename = Path.GetFileName(saveFileDialog.FileName);
                    }
                    var path = Path.Combine(selectedFolder, filename);
                    File.WriteAllBytes(path, entry.Data);
                }
            }
        }

        public void Add(string[] files)
        {
            var filenames = files
                    .Where(f => File.Exists(f));
            foreach (var filename in filenames)
            {
                var resourceName = Path.GetFileNameWithoutExtension(filename);
                resourceName = resourceName.Substring(0, Math.Min(8, resourceName.Length));
                var type = Path.GetExtension(filename).TrimStart('.');
                type = type.Substring(0, Math.Min(4, type.Length)).ToUpper();
                var data = File.ReadAllBytes(filename);
                Archive.AddEntry(resourceName, type, data);
            }
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(Archive));
        }

        private void Add()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                Add(openFileDialog.FileNames);
            }
        }

        private void Delete()
        {
            var selectedEntries = ViewCore.GetSelectedEntries();
            if (!selectedEntries.Any())
            {
                return;
            }
            foreach(var entry in selectedEntries)
            {
                Archive.DeleteEntry(entry);
            }
            RaisePropertyChanged(nameof(Archive));
        }

    }
}
