using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesSaveLocationTracker.Tes.Renderer;

namespace TesSaveLocationTracker.Tes.Skyrim
{
    public class SkyrimGameData : TesGameData
    {
        public override int GetDefaultWorldspace1FormID()
        {
            return 0x0000003c;
        }

        public override int GetDefaultWorldspace2FormID()
        {
            return 0x0000003c;
        }

        public override string GetGameSaveDirectory()
        {
            return
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd('\\') +
                "\\My Games\\Skyrim\\Saves";
        }

        public override TesSavegameRenderer GetRenderer(IList<SolidBrush> brushes)
        {
            return new TesSavegameRenderer(brushes)
            {
                TotalCellsX = 74 + 75,
                TotalCellsY = 50 + 49,
                CellOffsetX = 74,
                CellOffsetY = 50,
                CellSize = 4096.0d
            };
        }

        public override string GetSaveFileExtension()
        {
            return ".ess";
        }

        public override bool IsInDefaultWorldspace(int ws1FormID, int ws2FormID)
        {
            return (ws1FormID == GetDefaultWorldspace1FormID()) &&
                   (ws2FormID == GetDefaultWorldspace2FormID());
        }
    }
}
