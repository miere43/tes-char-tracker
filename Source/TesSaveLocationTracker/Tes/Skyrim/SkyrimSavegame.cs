using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesSaveLocationTracker.Tes.Skyrim
{
    /// <summary>
    /// Represent's Skyrim save file.
    /// </summary>
    public class SkyrimSavegame : TesSavegame
    {
        /// <summary>
        /// Gets a value indicating whether this instance is in Skyrim exterior worldspace.
        /// </summary>
        public bool IsInSkyrimWorldspace { get
            {
                return Worldspace1.FormID == 0x0000003c &&
                    Worldspace2.FormID == 0x0000003c;
            }
        }

        private static Image ReadScreenshot(BinaryReader reader)
        {
            int width = (int)reader.ReadUInt32();
            int height = (int)reader.ReadUInt32();
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte[] pixels = reader.ReadBytes(3);
                    bitmap.SetPixel(x, y, Color.FromArgb(pixels[0], pixels[1], pixels[2]));
                }
            }
            return bitmap;
        }

        public static SkyrimSavegame Parse(Stream input)
        {
            using (var reader = new TesSavegameReader(input, Encoding.ASCII))
            {
                byte[] magic = reader.ReadBytes(13);
                if (Encoding.ASCII.GetString(magic) != "TESV_SAVEGAME")
                    throw new ArgumentException(nameof(input) + " is not valid Skyrim savegame.");

                uint headerSize = reader.ReadUInt32();

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
