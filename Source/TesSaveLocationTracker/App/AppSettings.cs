using Newtonsoft.Json;
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

namespace TesSaveLocationTracker.App
{
    [Serializable]
    public class AppSettings
    {
        public float FirstDrawCircleRadius { get; set; } = 8.0f;

        public float DrawCircleRadius { get; set; } = 4.0f;

        public float LineSize { get; set; } = 2.0f;

        public string SkyrimSaveDir { get; set; }

        public string SkyrimMapFilePath  { get; set; } = "Resources/skyrim-map.jpg";

        public string Fallout4SaveDir { get; set; }

        public string Fallout4MapFilePath { get; set; } = "Resources/fallout4-8-map.jpg";

        public int LegendX { get; set; } = 1;

        public int LegendY { get; set; } = 1;

        public int LegendFontSize { get; set; } = 16;

        public FontStyle LegendFontStyle { get; set; } = FontStyle.Regular;

        public string LegendFontName { get; set; } = "Segoe UI";

        private static List<string> DrawColorsDefault = new List<string>()
        {
            "LightBlue",  "LightGreen", "LightPink", "LightCyan", "CornflowerBlue", "Orange"
        };

        public string Game { get; set; } = "Fallout 4";

        public List<string> DrawColorsStrings { get; set; } = new List<string>(0);

        public List<string> IgnoreCharacters = new List<string>(0);

        [NonSerialized]
        public List<SolidBrush> DrawColors = ParseBrushes(null, DrawColorsDefault, true);

        public AppSettings()
        {
        }

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists("settings.json"))
                    return new AppSettings();

                var s = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("settings.json"));
                if (s == null)
                    throw new ArgumentException("Settings file is not valid (probably empty)");

                if (s.DrawColorsStrings.Count == 0)
                {
                    s.DrawColorsStrings = DrawColorsDefault;
                }

                s.DrawColors = ParseBrushes(DrawColorsDefault, s.DrawColorsStrings, false);

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
                MessageBox.Show("Using default settings because of error during loading settings: " + e.Message);
                return new AppSettings();
            }
        }

        public void Save()
        {
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(this, Formatting.Indented));
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
