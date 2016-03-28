using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesSaveLocationTracker.Utility
{
    public static class SkyrimUtility
    {
        public static string GetSkyrimSaveDirectory()
        {
            // Path.Combine() works weird.
            return 
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd('\\') + 
                "\\My Games\\Skyrim\\Saves";
        }
    }
}
