using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesSaveLocationTracker.Tes
{
    public static class TesUtility
    {
        public static string GetSkyrimSaveDirectory()
        {
            // Path.Combine() works weird.
            return
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd('\\') +
                "\\My Games\\Skyrim\\Saves";
        }

        public static string GetFallout4SaveDirectory()
        {
            return
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd('\\') +
                "\\My Games\\Fallout4\\Saves";
        }
    }
}
