using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using TesSaveLocationTracker.Tes;
using TesSaveLocationTracker.Tes.Skyrim;

namespace TesSaveLocationTracker.Utility
{
    [Serializable]
    public class AppSettings
    {
        public float FirstDrawCircleRadius { get; private set; } = 8.0f;

        public float DrawCircleRadius { get; private set; } = 5.0f;

        public string SkyrimSaveDir { get; private set; } = "";

        public string SkyrimMapFilePath  { get; private set; } = "skyrim-map.jpg";

        public string Fallout4SaveDir { get; private set; } = "";

        public string Fallout4MapFilePath { get; private set; } = "fallout4-8-map.jpg";

        private static List<string> DrawColorsDefault = new List<string>()
        {
            "LightBlue",  "LightGreen", "LightPink", "LightCyan", "White"
        };

        public List<string> DrawColorsStrings
        {
            get
            {
                List<string> s = new List<string>();

                foreach (var color in DrawColors)
                {
                    if (color.Color.IsNamedColor)
                        s.Add(color.Color.Name);
                    else
                        s.Add(Convert.ToString(color.Color.ToArgb(), 16).Substring(2));
                }
                return s;
            }
            set
            {
                DrawColors = ParseBrushes(DrawColorsDefault, value, false);
            }
        }

        [NonSerialized]
        public List<SolidBrush> DrawColors = ParseBrushes(null, DrawColorsDefault, true);

        private static CultureInfo invariantCulture;

        private static IFormatProvider invariantNumberFormat;

        private static Ini s;

        public AppSettings()
        {
            invariantCulture = CultureInfo.InvariantCulture;
            invariantNumberFormat = invariantCulture.NumberFormat;
        }

        public static AppSettings Load()
        {
            try
            {
                var s = JObject.Parse(File.ReadAllText("settings.json")).ToObject<AppSettings>();
                if (!Directory.Exists(s.SkyrimSaveDir))
                {
                    s.SkyrimSaveDir = (new SkyrimGameData()).GetGameSaveDirectory();
                    if (!Directory.Exists(s.SkyrimSaveDir))
                    {
                        throw new ArgumentException("Cannot find Skyrim save directory or read it from settings file. Set "
                            + nameof(s.SkyrimSaveDir) + " setting in settings.json file.");
                    }
                }
                if (!File.Exists(s.SkyrimMapFilePath))
                {
                    throw new ArgumentException("Cannot find Skyrim map " + s.SkyrimMapFilePath + ". Set "
                        + nameof(s.SkyrimMapFilePath) + " setting in settings.json file.");
                }
                if (!Directory.Exists(s.Fallout4SaveDir))
                {
                    s.Fallout4SaveDir = (new SkyrimGameData()).GetGameSaveDirectory();
                    if (!Directory.Exists(s.Fallout4SaveDir))
                    {
                        throw new ArgumentException("Cannot find Fallout 4 save directory or read it from settings file. Set "
                            + nameof(s.Fallout4SaveDir) + " setting in settings.json file.");
                    }
                }
                if (!File.Exists(s.Fallout4MapFilePath))
                {
                    throw new ArgumentException("Cannot find Fallout 4 map " + s.Fallout4MapFilePath + ". Set "
                        + nameof(s.Fallout4MapFilePath) + " setting in settings.json file.");
                }

                return s;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error during loading settings: " + e.Message);
                return new AppSettings();
            }
        }

        public void Save()
        {
            File.WriteAllText("settings.json", JObject.FromObject(this).ToString());
        }

        private static float ParseFloat(float fallback, string value)
        {
            float val;
            if (float.TryParse(value, NumberStyles.Float, invariantNumberFormat, out val))
                return val;
            return fallback;
        }

        private static string ParsePath(string fallback, string value)
        {
            if (Directory.Exists(value))
                return value;
            return fallback;
        }
        
        private static string ParseFilePath(string fallback, string value)
        {
            if (File.Exists(value))
                return value;
            return fallback;
        }

        private static List<SolidBrush> ParseBrushes(List<string> fallback, List<string> value, bool fallingBack = false)
        {
            if (!fallingBack && value == null)
                return ParseBrushes(null, fallback, true);

            List<SolidBrush> brushes = new List<SolidBrush>();
            foreach (var color in value)
            {
                int val;
                if (int.TryParse(color,
                    NumberStyles.HexNumber, 
                    CultureInfo.InvariantCulture.NumberFormat, out val))
                {
                    // set alpha to 255
                    unchecked { val = val | (int)0xFF000000; }
                    brushes.Add(new SolidBrush(Color.FromArgb(val)));
                }
                else
                {
                    var brush = new SolidBrush(Color.FromName(color.Trim()));
                    if (brush.Color.A == 0 &&
                        brush.Color.B == 0 &&
                        brush.Color.G == 0 &&
                        brush.Color.R == 0)
                    {
                        MessageBox.Show("Cannot parse color " + color);
                    }
                    else
                        brushes.Add(brush);
                }
            }

            if (!fallingBack && brushes.Count == 0)
                return ParseBrushes(null, fallback, true);

            return brushes;
        }
    }
}
