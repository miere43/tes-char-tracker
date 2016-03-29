using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesSaveLocationTracker.Tes.Skyrim
{
    /// <summary>
    /// Represent's Skyrim save file.
    /// </summary>
    public class SkyrimSavegame
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
        /// Skyrim internal save date.
        /// </summary>
        public string SaveDate { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is in Skyrim exterior worldspace.
        /// </summary>
        public bool IsInSkyrimWorldspace { get
            {
                return Worldspace1.FormID == 0x0000003c &&
                    Worldspace2.FormID == 0x0000003c;
            }
        }

        public static SkyrimSavegame Parse(Stream input)
        {
            using (var reader = new TesSavegameReader(input, Encoding.ASCII))
            {
                byte[] magic = reader.ReadBytes(13);
                // Debug.WriteLine($"magic: {Encoding.ASCII.GetString(magic)}");

                uint headerSize = reader.ReadUInt32();
                // Debug.WriteLine($"header size: {headerSize}");

                uint saveVersion = reader.ReadUInt32();
                uint saveNumber = reader.ReadUInt32();
                string charName = reader.ReadWString();
                uint playerLevel = reader.ReadUInt32();
                string playerLocation = reader.ReadWString();
                string gameDate = reader.ReadWString();

                // playerRaceEditorId
                reader.ReadWString();
                // playerSex
                reader.ReadUInt16();
                // playerCurExp
                reader.ReadSingle();
                // playerLvlUpExp
                reader.ReadSingle();
                // filetime
                reader.ReadBytes(8);

                uint screenWidth = reader.ReadUInt32();
                uint screenHeight = reader.ReadUInt32();
                // Debug.WriteLine($"screenshot width:  {screenWidth}");
                // Debug.WriteLine($"screenshot height: {screenHeight}");

                // skip screenshot data
                input.Seek(3U * screenWidth * screenHeight, SeekOrigin.Current);

                byte gameVersion = reader.ReadByte();
                // Debug.WriteLine($"game version: {gameVersion}");

                uint pluginInfoSize = reader.ReadUInt32();
                // Debug.WriteLine($"plugin info size: {pluginInfoSize}");

                // skip plugin info
                input.Seek(pluginInfoSize, SeekOrigin.Current);

                // file location table
                uint formIDArrayCountOffset = reader.ReadUInt32();
                uint unknownTable3Offset = reader.ReadUInt32();
                uint globalDataTable1Offset = reader.ReadUInt32();
                uint globalDataTable2Offset = reader.ReadUInt32();
                uint changeFormsOffset = reader.ReadUInt32();
                uint globalDataTable3Offset = reader.ReadUInt32();
                uint globalDataTable1Count = reader.ReadUInt32();
                uint globalDataTable2Count = reader.ReadUInt32();
                uint globalDataTable3Count = reader.ReadUInt32();
                uint changeFormCount = reader.ReadUInt32();
                // unused uint32[15]

                // Debug.WriteLine($"table count: {globalDataTable1Count}");

                // move to globaldatatable1
                input.Seek(globalDataTable1Offset, SeekOrigin.Begin);

                UInt32 nextObjectId = 0;
                RefID worldSpace1 = null;
                int coorX = 0;
                int coorY = 0;
                RefID worldSpace2 = null;
                float posX = 0.0f;
                float posY = 0.0f;
                float posZ = 0.0f;
                byte unknown = 0;

                for (int i = 0; i < globalDataTable1Count; i++)
                {
                    uint type = reader.ReadUInt32();
                    uint length = reader.ReadUInt32();
                    if (type != 1)
                    {
                        input.Seek(length, SeekOrigin.Current);
                    }
                    else
                    {
                        nextObjectId = reader.ReadUInt32();
                        worldSpace1 = reader.ReadRefID();
                        coorX = reader.ReadInt32();
                        coorY = reader.ReadInt32();
                        worldSpace2 = reader.ReadRefID();
                        posX = reader.ReadSingle();
                        posY = reader.ReadSingle();
                        posZ = reader.ReadSingle();
                        unknown = reader.ReadByte();
                        break;
                    }
                }

                // weird code ahead
                input.Seek(formIDArrayCountOffset, SeekOrigin.Begin);
                uint formIDArrayCount = reader.ReadUInt32();
                if (worldSpace1.Type == RefIDType.FormID)
                {
                    if (worldSpace1.FormID < formIDArrayCount)
                        for (uint i = 1; i < formIDArrayCount; i++)
                        {
                            int ptrFormId = reader.ReadInt32();
                            if (i == worldSpace1.FormID)
                            {
                                worldSpace1.AssociatedFormID = ptrFormId;
                                break;
                            }
                        }
                }

                input.Seek(formIDArrayCountOffset, SeekOrigin.Begin);
                reader.ReadUInt32();
                if (worldSpace2.Type == RefIDType.FormID ||
                    worldSpace2.Type == RefIDType.Skyrim)
                {
                    if (worldSpace2.FormID < formIDArrayCount)
                        for (uint i = 1; i < formIDArrayCount; i++)
                        {
                            int ptrFormId = reader.ReadInt32();
                            if (i == worldSpace2.FormID)
                            {
                                worldSpace2.AssociatedFormID = ptrFormId;
                                break;
                            }
                        }
                }

                return new SkyrimSavegame()
                {
                    NextObjectID = nextObjectId,
                    X = posX,
                    Y = posY,
                    Z = posZ,
                    CellX = coorX,
                    CellY = coorY,
                    Worldspace1 = worldSpace1,
                    Worldspace2 = worldSpace2,
                    CharacterName = charName,
                    CharacterLevel = (int)playerLevel,
                    CharacterLocationName = playerLocation,
                    SaveDate = gameDate,
                    SaveNumber = (int)saveNumber
                };
            }
        }
    }
}
