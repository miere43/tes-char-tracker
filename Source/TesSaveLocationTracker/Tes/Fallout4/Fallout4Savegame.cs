using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesSaveLocationTracker.Tes.Fallout4
{
    public class Fallout4Savegame : TesSavegame
    {
        public static Fallout4Savegame Parse(Stream input)
        {
            using (var reader = new TesSavegameReader(input))
            {
                byte[] magic = reader.ReadBytes(12); // FO4_SAVEGAME
                if (Encoding.ASCII.GetString(magic) != "FO4_SAVEGAME")
                    throw new ArgumentException(nameof(input) + " is not valid Fallout 4 savegame.");

                uint headerSize = reader.ReadUInt32(); // 100

                // HEADER
                uint saveVersion = reader.ReadUInt32(); // 12
                uint saveNumber = reader.ReadUInt32(); // 1, 2, 3
                string charName = reader.ReadUTF8WString(); // "HONEY"
                uint playerLevel = reader.ReadUInt32(); // 1
                string playerLocation = reader.ReadUTF8WString(); // "Commonwealth"
                string gameDate = reader.ReadUTF8WString(); // 0d.0h.8m.0 days.0 hours.8 minutes"

                string playerRaceEditorId = reader.ReadWString(); // HumanRace
                ushort playerSex = reader.ReadUInt16(); // 1 - female, 0 - male
                float playerCurExp = reader.ReadSingle(); // 0, 72
                float playerLvlUpExp = reader.ReadSingle(); // 200
                // filetime
                reader.ReadBytes(8);

                uint screenWidth = reader.ReadUInt32();
                uint screenHeight = reader.ReadUInt32();

                // skip screenshot data
                input.Seek(4U * screenWidth * screenHeight, SeekOrigin.Current);

                byte gameVersion = reader.ReadByte(); // 62
                string gameVersionString = reader.ReadWString();

                uint pluginInfoSize = reader.ReadUInt32();
                input.Seek(pluginInfoSize, SeekOrigin.Current);
                //byte pluginsCount = reader.ReadByte();
                //string[] plugins = new string[pluginsCount];
                //for (int i = 0; i < pluginsCount; i++)
                //    plugins[i] = reader.ReadWString();

                //
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

                //uint[] unusedLocations = new uint[16];
                //for (int i = 0; i < unusedLocations.Length; i++)
                //    unusedLocations[i] = reader.ReadUInt32();

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

                return new Fallout4Savegame()
                {
                    X = posX,
                    Y = posY,
                    Z = posZ,
                    SaveNumber = (int)saveNumber,
                    SaveDate = gameDate,
                    Worldspace1 = worldSpace1,
                    Worldspace2 = worldSpace2,
                    CellX = coorX,
                    CellY = coorY,
                    CharacterLevel = (int)playerLevel,
                    CharacterName = charName,
                    CharacterLocationName = playerLocation,
                    NextObjectID = nextObjectId
                };
            }
        }
    }
}
