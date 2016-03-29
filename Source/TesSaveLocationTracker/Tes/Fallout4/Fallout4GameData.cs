using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesSaveLocationTracker.Tes.Renderer;

namespace TesSaveLocationTracker.Tes.Fallout4
{
    public class Fallout4GameData : TesGameData
    {
        /// <summary>
        /// Get's Commonwealth worldspace FormID.
        /// </summary>
        public override int GetDefaultWorldspace1FormID()
        {
            return 0x0000003c;
        }

        /// <summary>
        /// Get's Commonwealth worldspace FormID.
        /// </summary>
        public override int GetDefaultWorldspace2FormID()
        {
            return 0x0000003c;
        }

        public override string GetGameSaveDirectory()
        {
            return
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd('\\') +
                "\\My Games\\Fallout4\\Saves";
        }

        public override TesSavegameRenderer GetRenderer(IList<SolidBrush> brushes)
        {
            return new TesSavegameRenderer(brushes)
            {
                CellSize = 4096.0d,
                TotalCellsX = 24 + 24,
                TotalCellsY = 24 + 24,
                CellOffsetX = 24,
                CellOffsetY = 24
            };
        }

        public override bool IsInDefaultWorldspace(int ws1FormID, int ws2FormID)
        {
            return (ws1FormID == GetDefaultWorldspace1FormID()) &&
                   (ws2FormID == GetDefaultWorldspace2FormID());
        }
    }
}
