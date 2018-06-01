using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LfdArchiver.Domain
{
    public class ResourceEntry : INotifyPropertyChanged
    {
        public static Dictionary<string, string> descriptions = new Dictionary<string, string>
        {
            { "CRFT", "X-Wing Floppy 3D model" },
            { "CPLX", "X-Wing CD 3D model" },
            { "XACT", "X-Wing high resolution bitmap data" },
            { "VOIC", "Creative Voice Audio" },
            { "TEXT", "String table" },
            { "BLAS", "Creative Voice Audio" },
            { "PLTT", "Indexed color palette" },
            { "FILM", "Landru cutscene script" },
            { "ANIM", "Animated sprite" },
            { "DELT", "Static sprite" },
            { "FONT", "Bitmap font" },
            { "PANL", "Cockpit sprite data" },
            { "RMAP", "Resource archive contents table" },
            { "BMAP", "Bitmap data" },
            { "CUST", "Bitmap data" },
            { "GMID", "General MIDI music file" },
            { "MASK", "Cockpit transparency mask" },
            { "SHIP", "TIE Fighter'94-95 craft model format" },
            { "ADLB", "AdLib audio file" },
            { "RLND", "Roland audio file" }
        };

        public LfdHeader Header;

        protected byte[] m_data;

        public event PropertyChangedEventHandler PropertyChanged;

        public byte[] Data
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
                Header.Length = value.Length;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Length)));
            }
        }

        public string Name
        {
            get
            {
                return Header.Name;
            }

            set
            {
                Header.Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public string Type
        {
            get
            {
                return Header.Type;
            }

            set
            {
                Header.Type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }

        public int Length
        {
            get
            {
                return Header.Length;
            }
        }

        public string Description
        {
            get
            {
                if (descriptions.ContainsKey(Type))
                {
                    return descriptions[Type];
                }
                else
                {
                    return "Unknown resource";
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return Header.IsValid;
            }
        }

        public static ResourceEntry Load(Stream stream)
        {
            var entry = new ResourceEntry();
            entry.Header = stream.ReadStruct<LfdHeader>();
            entry.LoadData(stream);
            return entry;
        }

        public void LoadData(Stream stream)
        {
            Data = new byte[Header.Length];
            stream.Read(Data, 0, Header.Length);            
        }

        public void Write(Stream stream)
        {
            Header.Write(stream);
            stream.Write(Data, 0, Header.Length);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1} ({2})", Name, Type, Length);
        }
    }
}
