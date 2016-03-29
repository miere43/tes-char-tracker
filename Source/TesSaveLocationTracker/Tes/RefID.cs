using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesSaveLocationTracker.Tes
{
    /// <summary>
    /// Represent's three-byte sequence, which representing
    /// packed FormID.
    /// </summary>
    public class RefID
    {
        public byte[] Bytes { get; internal set; }

        /// <summary>
        /// FormID from default master file (Skyrim.esm).
        /// Use only when RefID.Type is Skyrim.
        /// </summary>
        public int FormID
        {
            get
            {
                // get rid of two upper bits
                // bytes[0] is implicitly converted to int.
                // so we shifting int.
                byte c = (byte)((Bytes[0] << 26) >> 26);
                int formId = BitConverter.ToInt32(new byte[] { Bytes[2], Bytes[1], c, 0 }, 0);
                return formId;
            }
        }

        /// <summary>
        /// FormID from save game's FormID array. Use only when
        /// RefID.Type is FormID.
        /// </summary>
        public int AssociatedFormID { get; internal set; }

        /// <summary>
        /// RefID type.
        /// </summary>
        public RefIDType Type
        {
            get
            {
                return (RefIDType)(Bytes[0] >> 6);
            }
        }
    }
}
