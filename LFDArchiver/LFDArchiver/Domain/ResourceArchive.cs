using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace LfdArchiver.Domain
{
    public class ResourceArchive : INotifyPropertyChanged
    {
        public bool HasTableOfContents { get; set; }

        public string Path { get; set; }

        public ObservableCollection<ResourceEntry> Entries { get; set; }

        public bool IsChanged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanBeSaved
        {
            get
            {
                return IsChanged && File.Exists(Path);
            }
        }

        public ResourceArchive()
        {
            Entries = new ObservableCollection<ResourceEntry>();
            Path = "";
            IsChanged = false;
            HasTableOfContents = true;
        }

        public void Load(string path)
        {
            Path = path;
            using (var stream = File.OpenRead(path))
            {
                Load(stream);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanBeSaved)));
        }

        public void Load(Stream stream)
        {
            var header = LfdHeader.Read(stream);
            HasTableOfContents = (header.Type == "RMAP");

            Entries.Clear();

            ResourceEntry entry = new ResourceEntry();
            if (HasTableOfContents)
            {
                var count = header.Length / 16;
                var headers = new List<LfdHeader>(count);
                for (var i = 0; i < count; i++)
                {
                    headers.Add(LfdHeader.Read(stream));
                }
                entry = ResourceEntry.Load(stream);
            }
            else
            {
                entry.Header = header;
                entry.LoadData(stream);
            }
             
            while (entry.IsValid)
            {                                
                Entries.Add(entry);
                entry = ResourceEntry.Load(stream);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Entries)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasTableOfContents)));
        }

        public void Save(Stream stream)
        {
            if (HasTableOfContents)
            {
                var header = new LfdHeader
                {
                    Type = "RMAP",
                    Name = "resource",
                    Length = Entries.Count * 16
                };
                header.Write(stream);
            
            foreach (var entry in Entries)
            {
                entry.Header.Write(stream);
            }
            }
            foreach (var entry in Entries)
            {
                entry.Header.Write(stream);
                stream.Write(entry.Data, 0, entry.Data.Length);
            }
            IsChanged = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChanged)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanBeSaved)));
        }

        public void Save(string path)
        {
            Path = path;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path)));
            using (var stream = File.OpenWrite(path))
            {
                Save(stream);
            }
        }

        public void Save()
        {
            Save(Path);
        }

        public void AddEntry(string name, string type, byte[] data)
        {
            var entry = Entries.FirstOrDefault(e => e.Name == name && e.Type == type);
            if (entry == null)
            {
                entry = new ResourceEntry()
                {
                    Name = name,
                    Type = type,
                    Data = data
                };
                Entries.Add(entry);
            }
            else
            {
                entry.Data = data;
            }
            IsChanged = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChanged)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Entries)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanBeSaved)));
        }

        public void DeleteEntry(ResourceEntry entry)
        {
            Entries.Remove(entry);
            IsChanged = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChanged)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Entries)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanBeSaved)));
        }                
    }
}
