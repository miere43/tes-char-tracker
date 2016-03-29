﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TesSaveLocationTracker.Tes;
using TesSaveLocationTracker.Tes.Fallout4;
using TesSaveLocationTracker.Tes.Skyrim;

namespace TesSaveLocationTracker.App
{
    public partial class MainForm : Form
    {
        AppSettings settings;

        Image renderedImage;

        TesGameData gameData;

        public MainForm()
        {
            InitializeComponent();

            Load += MainForm_Load;
        }

        private void MainForm_Load(object unused, EventArgs unused1)
        {
            settings = AppSettings.Load();

            IEnumerable<string> savePaths;

            List<TesSavegame> games = new List<TesSavegame>();

            string mapPath = null;

            if (settings.Game.Trim() == "Fallout 4")
            {
                gameData = new Fallout4GameData();
                mapPath = settings.Fallout4MapFilePath;
                savePaths = Directory.EnumerateFiles(gameData.GetGameSaveDirectory() + "\\", "*" + gameData.GetSaveFileExtension());

                foreach (var savePath in savePaths)
                {
                    using (var stream = File.OpenRead(savePath))
                    {
                        Fallout4Savegame save = Fallout4Savegame.Parse(stream);
                        if (settings.IgnoreCharacters.Contains(save.CharacterName.Trim()))
                            continue;
                        games.Add(save);
                    }
                }
            }
            else if (settings.Game.Trim() == "Skyrim")
            {
                gameData = new SkyrimGameData();
                mapPath = settings.SkyrimMapFilePath;
                savePaths = Directory.EnumerateFiles(gameData.GetGameSaveDirectory() + "\\", "*" + gameData.GetSaveFileExtension());

                foreach (var savePath in savePaths)
                {
                    using (var stream = File.OpenRead(savePath))
                    {
                        SkyrimSavegame save = SkyrimSavegame.Parse(stream);
                        if (settings.IgnoreCharacters.Contains(save.CharacterName.Trim()))
                            continue;
                        games.Add(save);
                    }
                }
            }
            else
            {
                MessageBox.Show("Game " + settings.Game + " is not supported.");
                Environment.Exit(-1);
            }

            renderedImage = Image.FromFile(mapPath); // AppSettings.SkyrimMapFilePath);

            var screen = Screen.PrimaryScreen.Bounds;
            int formWidth = renderedImage.Width > (screen.Width * 0.9) ? (int)(screen.Width * 0.9) : renderedImage.Width;
            int formHeight = renderedImage.Height > (screen.Height * 0.9) ? (int)(screen.Height * 0.9) : renderedImage.Height;

            this.Width = formWidth;
            this.Height = formHeight;

            var renderer = gameData.GetRenderer(settings.DrawColors);

            renderer.DrawCircleRadius = settings.DrawCircleRadius;
            renderer.FirstDrawCircleRadius = settings.FirstDrawCircleRadius;
            renderer.LegendX = 1;
            renderer.LegendY = 1;
            renderer.GameData = gameData;

            var graphics = renderer.Render(renderedImage, games);

            SaveMapButton.Location = new Point(10, (int)renderer.LegendEndY);

            graphics.Flush();
            graphics.Dispose();

            PictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            PictureBox.Image = renderedImage;
            PictureBox.Width = renderedImage.Width;
            PictureBox.Height = renderedImage.Height;

            this.CenterToScreen();
        }

        protected override void OnClosed(EventArgs e)
        {
            settings.Save();
        }

        private void SaveMapButton_Click(object sender, EventArgs unused)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            string ext = Path.GetExtension(settings.SkyrimMapFilePath);
            bool hasExt = ext != "";

            dialog.FileName = Path.GetFileNameWithoutExtension(settings.SkyrimMapFilePath)
                + "-tracked" + ext;
            if (hasExt)
                dialog.Filter = ext.Substring(1).ToUpper() + "|*" + ext + "|All files|*.*";
            else
                dialog.Filter = "All files|*.*|";
            dialog.CreatePrompt = true;
            dialog.Title = "Save tracked image as...";
            dialog.ShowDialog();

            try
            {
                File.Create(dialog.FileName).Dispose();
                renderedImage.Save(dialog.FileName);
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot save image: " + e.Message);
            }
        }
    }
}
