using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesSaveLocationTracker.Tes
{
    public class TesSavegameReader : BinaryReader
    {
        private static Encoding readWStringEncoding;

        static TesSavegameReader()
        {
            try
            {
                // Get Windows-1252 encoding.
                readWStringEncoding = Encoding.GetEncoding(1252);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(NotSupportedException) ||
                    e.GetType() == typeof(ArgumentException))
                {
                    readWStringEncoding = Encoding.ASCII;
                }
                else
                {
                    throw;
                }
            }
        }

        public TesSavegameReader(Stream input)
            : base(input)
        {

        }

        public TesSavegameReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {

        }

        /// <summary>
        /// Reads the WString (length-prefixed Windows-1252 string).
        /// </summary>
        public string ReadWString()
        {
            ushort length = this.ReadUInt16();
            return readWStringEncoding.GetString(this.ReadBytes(length));
        }

        public string ReadUTF8WString()
        {
            ushort length = this.ReadUInt16();
            return Encoding.UTF8.GetString(this.ReadBytes(length));
        }

        /// <summary>
        /// Reads the RefID and advances stream by 3 bytes.
        /// </summary>
        public RefID ReadRefID()
        {
            return new RefID()
            {
                Bytes = this.ReadBytes(3)
            };
        }
    }
}
