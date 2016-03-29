using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesSaveLocationTracker.Tes.Renderer;

namespace TesSaveLocationTracker.Tes
{
    public abstract class TesGameData
    {
        public abstract string GetGameSaveDirectory();

        public abstract int GetDefaultWorldspace1FormID();

        public abstract int GetDefaultWorldspace2FormID();

        public abstract bool IsInDefaultWorldspace(int ws1FormID, int ws2FormID);

        public abstract TesSavegameRenderer GetRenderer(IList<Brush> brushes);
    }
}
