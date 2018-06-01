using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LfdArchiver.Domain
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct LfdHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        byte[] m_type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        byte[] m_name;
        public Int32 Length;

        public string Type
        {
            get
            {
                return new string(Encoding.ASCII.GetChars(m_type.TakeWhile(b => b != 0).ToArray())).Trim();
            }
            set
            {
                var bytes = Encoding.ASCII.GetBytes(new string(value.Take(4).ToArray()));
                m_type = new byte[4];
                Array.Copy(bytes, m_type, bytes.Length);
            }
        }

        public string Name
        {
            get
            {
                return new string(Encoding.ASCII.GetChars(m_name.TakeWhile(b => b != 0).ToArray())).Trim();
            }
            set
            {
                var bytes = Encoding.ASCII.GetBytes(new string(value.Take(8).ToArray()));
                m_name = new byte[8];
                Array.Copy(bytes, m_name, bytes.Length);
            }
        }
        
        public bool IsValid
        {
            get
            {
                return m_name != null && m_type != null;
            }
        }

        public static LfdHeader Read(Stream stream)
        {
            return stream.ReadStruct<LfdHeader>();
        }

        public void Write(Stream stream)
        {
            stream.WriteStruct(this);
        }
    }
}
