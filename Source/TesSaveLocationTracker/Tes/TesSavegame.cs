using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesSaveLocationTracker.Tes
{
    public abstract class TesSavegame
    {
        /// <summary>
        /// Player X position in interior or worldspace (exterior).
        /// </summary>
        public float X { get; internal set; }

        /// <summary>
        /// Player Y position in interior or worldspace (exterior).
        /// </summary>
        public float Y { get; internal set; }

        /// <summary>
        /// Player Z position in interior or worldspace (exterior).
        /// </summary>
        public float Z { get; internal set; }

        /// <summary>
        /// Player worldspace (exterior) cell X. Interior's doesn't have cells.
        /// </summary>
        public int CellX { get; internal set; }

        /// <summary>
        /// Player worldspace (exterior) cell Y. Interior's doesn't have cells.
        /// </summary>
        public int CellY { get; internal set; }

        /// <summary>
        /// Save number.
        /// </summary>
        public int SaveNumber { get; internal set; }

        /// <summary>
        /// Player level.
        /// </summary>
        public int CharacterLevel { get; internal set; }

        /// <summary>
        /// Player-associated worldspace 1 RefID.
        /// </summary>
        public RefID Worldspace1 { get; internal set; }

        /// <summary>
        /// Player-associated worldspace 2 RefID.
        /// </summary>
        public RefID Worldspace2 { get; internal set; }

        /// <summary>
        /// Unique next savegame ID.
        /// </summary>
        public uint NextObjectID { get; internal set; }

        /// <summary>
        /// Player name.
        /// </summary>
        public string CharacterName { get; internal set; }

        /// <summary>
        /// Name of location where player currently at.
        /// </summary>
        public string CharacterLocationName { get; internal set; }

        /// <summary>
        /// Game internal save date.
        /// </summary>
        public string SaveDate { get; internal set; }
    }
}
