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

namespace TesSaveLocationTracker.Utility
{
    public static class AppSettings
    {
        private static string FirstDrawCircleRadiusKey = "fFirstDrawCircleRadius";
        private static float FirstDrawCircleRadiusDefault = 8.0f;
        public static float FirstDrawCircleRadius
            { get; private set; } = FirstDrawCircleRadiusDefault;

        private static string DrawCircleRadiusKey = "fDrawCircleRadius";
        private static float DrawCircleRadiusDefault = 5.0f;
        public static float DrawCircleRadius
            { get; private set; } = DrawCircleRadiusDefault;

        private static string SkyrimSaveDirKey = "sSkyrimSaveDir";
        private static string SkyrimSaveDirDefault = "";
        public static string SkyrimSaveDir
            { get; private set; } = SkyrimSaveDirDefault;

        private static string SkyrimMapFilePathKey = "sSkyrimMapFilePath";
        private static string SkyrimMapFilePathDefault = "skyrim-map.jpg";
        public static string SkyrimMapFilePath
            { get; private set; } = SkyrimMapFilePathDefault;

        private static string DrawColorsKey = "sDrawColors";
        private static string DrawColorsDefault = "LightBlue,LightGreen,LightPink,LightCyan,White,EDC9B9";
        private static string DrawColorsParsed = "";
        public static List<Brush> DrawColors
            { get; private set; } = new List<Brush>();

        private static CultureInfo invariantCulture;

        private static IFormatProvider invariantNumberFormat;

        private static Ini s;

        static AppSettings()
        {
            invariantCulture = CultureInfo.InvariantCulture;
            invariantNumberFormat = invariantCulture.NumberFormat;
        }

        public static bool ParseDefault()
        {
            var settings = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "settings.ini");

            return Parse(settings);
        }

        /// <summary>
        /// Can quit the application!
        /// </summary>
        public static bool Parse(string path)
        {
            if (!File.Exists(path))
                File.Create(path).Dispose();

            s = new Ini(path);

            FirstDrawCircleRadius = ParseFloat(FirstDrawCircleRadiusDefault, s.GetValue(FirstDrawCircleRadiusKey));
            DrawCircleRadius = ParseFloat(DrawCircleRadiusDefault, s.GetValue(DrawCircleRadiusKey));
            SkyrimSaveDir = ParsePath(SkyrimSaveDirDefault, s.GetValue(SkyrimSaveDirKey));
            if (SkyrimSaveDir.Trim() == SkyrimSaveDirDefault)
            {
                SkyrimSaveDir = SkyrimUtility.GetSkyrimSaveDirectory();
                if (!Directory.Exists(SkyrimSaveDir))
                {
                    MessageBox.Show("Cannot find Skyrim save directory or read it from settings file. Set "
                        + SkyrimSaveDirKey + " setting in settings.ini file.");
                    return false;
                }
            }
            SkyrimMapFilePath = ParseFilePath(SkyrimMapFilePathDefault, s.GetValue(SkyrimMapFilePathKey));
            if (!File.Exists(SkyrimMapFilePath))
            {
                MessageBox.Show("Cannot find Skyrim map image (" + SkyrimMapFilePath + ")");
                return false;
            }
            DrawColorsParsed = s.GetValue(DrawColorsKey, "", DrawColorsDefault);
            DrawColors = ParseBrushes(DrawColorsDefault, DrawColorsParsed, false);

            return true;
        }

        public static void Save()
        {
            if (s.GetValue(FirstDrawCircleRadiusKey) == "")
                s.WriteValue(FirstDrawCircleRadiusKey, FirstDrawCircleRadiusDefault.ToString(invariantCulture));
            if (s.GetValue(DrawCircleRadiusKey) == "")
                s.WriteValue(DrawCircleRadiusKey, DrawCircleRadiusDefault.ToString(invariantCulture));
            if (s.GetValue(SkyrimSaveDirKey) == "")
                s.WriteValue(SkyrimSaveDirKey, SkyrimSaveDirDefault);
            if (s.GetValue(SkyrimMapFilePathKey) == "")
                s.WriteValue(SkyrimMapFilePathKey, SkyrimMapFilePathDefault);
            if (s.GetValue(DrawColorsKey) == "")
                s.WriteValue(DrawColorsKey, DrawColorsParsed);

            s.Save();
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

        private static List<Brush> ParseBrushes(string fallback, string value, bool fallingBack = false)
        {
            if (!fallingBack && string.IsNullOrWhiteSpace(value))
                return ParseBrushes(null, fallback, true);

            List<Brush> brushes = new List<Brush>();
            string[] colors = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var color in colors)
            {
                int val;
                if (int.TryParse(color,
                    NumberStyles.HexNumber, 
                    invariantCulture.NumberFormat, out val))
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

            if (brushes.Count == 0)
                return ParseBrushes(null, fallback, true);

            return brushes;
        }
    }
}
