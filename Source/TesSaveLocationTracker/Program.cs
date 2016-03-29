using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TesSaveLocationTracker.Tes.Fallout4;
using TesSaveLocationTracker.Tes.Skyrim;
using TesSaveLocationTracker.Utility;

namespace TesSaveLocationTracker
{
    class Program
    {
        static IEnumerable<SkyrimSavegame> GetGames(int limit)
        {
            List<SkyrimSavegame> a = new List<SkyrimSavegame>();

            foreach (var d in Directory.EnumerateFiles(@"C:\Users\Vladislav\Documents\My Games\Skyrim\Saves\", "*.ess").Take(limit))
            {
                using (var stream = File.Open(d, FileMode.Open, FileAccess.Read))
                {
                    SkyrimSavegame save = SkyrimSavegame.Parse(stream);
                    a.Add(save);
                }
            }

            return a.OrderBy((save) => save.SaveNumber);
        }

        [STAThread]
        static int Main(string[] unused)
        {
            var settings = AppSettings.Load();

            //string mapfile = "D:/skyrim-map.jpg";
            //List<SkyrimSavegame> games = new List<SkyrimSavegame>();
            //string[] saves = new[]
            //{
            //    "quicksave.ess",
            //};
            //SkyrimGameData data = new SkyrimGameData();

            //foreach (var save in saves)
            //{
            //    using (var stream = File.OpenRead(data.GetGameSaveDirectory() + "\\" + save))
            //    {
            //        games.Add(SkyrimSavegame.Parse(stream));
            //    }
            //}

            string mapfile = "D:/fallout-16-map.jpg";

            Fallout4GameData data = new Fallout4GameData();
            var saves = Directory.EnumerateFiles(data.GetGameSaveDirectory() + "\\", "*.fos");
            List<Fallout4Savegame> games = new List<Fallout4Savegame>();
            foreach (var save in saves)
            {
                using (var stream = File.OpenRead(save))
                {
                    games.Add(Fallout4Savegame.Parse(stream));
                }
            }

            //if (!AppSettings.ParseDefault()) {
            //    return -1;
            //}

            //List<SkyrimSavegame> games = new List<SkyrimSavegame>();
            //foreach (var save in Directory.EnumerateFiles(AppSettings.SkyrimSaveDir, "*.ess"))
            //{
            //    //if (save.EndsWith("quicksave.ess"))
            //    //    continue;

            //    using (var stream = File.OpenRead(save))
            //    {
            //        games.Add(SkyrimSavegame.Parse(stream));
            //    }
            //}

            var image = Image.FromFile(mapfile); // AppSettings.SkyrimMapFilePath);

            var screen = Screen.PrimaryScreen.Bounds;
            int formWidth = image.Width > (screen.Width * 0.9) ? (int)(screen.Width * 0.9) : image.Width;
            int formHeight = image.Height > (screen.Height * 0.9) ? (int)(screen.Height * 0.9) : image.Height;

            Form form = new Form();
            form.Text = "TES Character Tracker";
            form.Width = formWidth;
            form.Height = formHeight;
            form.AutoScroll = true;
            form.MaximumSize = new Size(image.Width, image.Height);

            PictureBox graphicsBox = new PictureBox();
            var renderer = data.GetRenderer(settings.DrawColors);

            renderer.DrawCircleRadius = 8.0f;
            renderer.FirstDrawCircleRadius = 5.0f;
            renderer.LegendX = 1;
            renderer.LegendY = 1;
            renderer.GameData = data;

            var graphics = renderer.Render(image, games);

            Button saveToDiskButton = new Button();
            saveToDiskButton.Text = "Save map";
            saveToDiskButton.Width = 80;
            saveToDiskButton.Height = 40;
            saveToDiskButton.Location = new Point(10, (int)renderer.LegendEndY);

            graphics.Flush();
            graphics.Dispose();

            graphicsBox.SizeMode = PictureBoxSizeMode.AutoSize;
            graphicsBox.Image = image;
            graphicsBox.Width = image.Width;
            graphicsBox.Height = image.Height;

            saveToDiskButton.Click += (sender, args) =>
            {
                SaveFileDialog dialog = new SaveFileDialog();
                string ext = Path.GetExtension(settings.SkyrimMapFilePath);
                bool hasExt = ext != "";

                dialog.FileName = Path.GetFileNameWithoutExtension(settings.SkyrimMapFilePath)
                    + "-tracked" + ext;
                if (hasExt)
                    dialog.Filter = ext.Substring(1).ToUpper() + "|*" + ext + "|All files|*.*";
                else
                    dialog.Filter = "All files|*.*";
                dialog.CreatePrompt = true;
                dialog.Title = "Save tracked image as...";
                dialog.ShowDialog();

                try
                {
                    File.Create(dialog.FileName).Dispose();
                    image.Save(dialog.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Cannot save image: " + e.Message);
                }
            };

            form.Controls.Add(saveToDiskButton);
            form.Controls.Add(graphicsBox);
            form.ShowDialog();

            settings.Save();

            return 0;
        }

        private static ImageCodecInfo GetEncoderForExtension(string ext)
        {
            string extTrimmed = ext.TrimStart(new char[] { '.' });

            var encoders = ImageCodecInfo.GetImageEncoders();
            foreach (var encoder in encoders)
            {
                if (encoder.FilenameExtension.Contains(extTrimmed))
                {
                    return encoder;
                }
            }

            return null;
        }

        #region Debugging

        //static byte[] GetRefIDBytes(int refID)
        //{
        //    var bytes = BitConverter.GetBytes(refID);
        //    return new byte[] { bytes[0], bytes[1], bytes[2], 0 };
        //}

        //static void PrintHexArray(byte[] arr, int padLeft)
        //{
        //    string s = new string('0', (padLeft - arr.Length) * 2);
        //    foreach (var b in arr)
        //    {
        //        s += Convert.ToString(b, 16).PadLeft(2, '0');
        //        s += " ";
        //    }
        //    Console.WriteLine(s);
        //}

        //static void PrintBinaryArray(byte[] arr)
        //{
        //    foreach (var b in arr)
        //        Console.Write(Convert.ToString(b, 2).PadLeft(8, '0') + " ");
        //    Console.Write('\n');
        //}

        //public static void PrintBinaryInt(int a)
        //{
        //    var ar = BitConverter.GetBytes(a).Reverse();

        //    foreach (var s in ar)
        //        Console.Write(Convert.ToString(s, 2).PadLeft(8, '0') + " ");
        //    Console.WriteLine();
        //    foreach (var s in ar)
        //        Console.Write(s.ToString("X2") + " ");
        //    Console.WriteLine();
        //}


        //static void DebugSave(SkyrimSavegame game)
        //{

        //    StringBuilder s = new StringBuilder();
        //    s.Append("Cell: ");
        //    s.Append(game.CellX);
        //    s.Append(" ");
        //    s.Append(game.CellY);
        //    s.Append(" | ");
        //    s.Append(game.X);
        //    s.Append(" ");
        //    s.Append(game.Y);
        //    s.Append(" ");
        //    s.Append(game.Z);
        //    s.Append("IsExterior: ");
        //    s.Append(game.IsInSkyrimWorldspace);
        //    s.Append(" | ");
        //    s.Append(game.CharacterLocationName);

        //    Console.WriteLine(s.ToString());
        //}

        //static void Watcher()
        //{
        //    var watcher = new FileSystemWatcher(@"C:\Users\Vladislav\Documents\My Games\Skyrim\Saves");
        //    bool nextIsQuicksave = false;

        //    while (true)
        //    {
        //        var result = watcher.WaitForChanged(WatcherChangeTypes.Created | WatcherChangeTypes.Changed);

        //        bool next = false;
        //        if (result.Name == "quicksave.ess.tmp")
        //        {
        //            if (result.ChangeType == WatcherChangeTypes.Created)
        //                continue;

        //            Debug.WriteLine($"{result.Name} - {result.ChangeType}");

        //            const string exceptedName = @"C:\Users\Vladislav\Documents\My Games\Skyrim\Saves\quicksave.ess";
        //            Thread.Sleep(500);

        //            if (next)
        //                continue;

        //            using (var stream = File.Open(exceptedName, FileMode.Open, FileAccess.Read))
        //            {
        //                SkyrimSavegame save = SkyrimSavegame.Parse(stream);
        //                DebugSave(save);
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}
