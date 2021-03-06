﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesSaveLocationTracker.Tes.Skyrim;

namespace TesSaveLocationTracker.Tes.Renderer
{
    public class TesSavegameRenderer
    {
        public IList<SolidBrush> DrawBrushes { get; protected set; }

        public float FirstDrawCircleRadius { get; set; }

        public float DrawCircleRadius { get; set; }

        public float LineSize { get; set; }

        public float LegendX { get; set; }

        public float LegendY { get; set; }

        public float LegendEndY { get; private set; }

        public int LegendFontSize { get; set; }

        public int CellOffsetX { get; set; }

        public int CellOffsetY { get; set; }

        public int TotalCellsX { get; set; }

        public int TotalCellsY { get; set; }

        public double CellSize { get; set; }

        public string LegendFont { get; set; }

        public FontStyle LegendFontStyle { get; set; }

        public TesGameData GameData { get; set; }

        public InteriorDB InteriorDB { get; set; }

        public TesSavegameRenderer(IList<SolidBrush> brushes)
        {
            if (brushes == null)
                throw new ArgumentNullException(nameof(brushes));
            this.DrawBrushes = brushes;
            this.LegendFontSize = 16;
            this.LegendX = 1;
            this.LegendY = 1;
            this.FirstDrawCircleRadius = 8.0f;
            this.DrawCircleRadius = 5.0f;
            this.LineSize = 2.0f;
            this.LegendFontSize = 16;
            this.LegendFont = "Segoe UI";
            this.LegendFontStyle = FontStyle.Regular;
        }

        private Brush GetBrushByIndex(int index)
        {
            if (index >= DrawBrushes.Count)
                return Brushes.LightGray;
            return DrawBrushes[index];
        }

        public Graphics Render(Image fullGameMap, IEnumerable<TesSavegame> saves)
        {
            if (GameData == null)
                throw new ArgumentNullException("GameData property is not set.");

            var groupedSaves = saves.GroupBy((save) => save.CharacterName);

            // see index++
            int index = 0;
            IEnumerable<CharacterSaves> savesForChar = groupedSaves.Select((charSaves) =>
            {
                return new CharacterSaves()
                {
                    Saves = charSaves.OrderBy((sg) => sg.SaveNumber),
                    CharacterName = charSaves.First().CharacterName,
                    Brush = GetBrushByIndex(index++)
                };
            });

            float posCircleRadius = this.DrawCircleRadius;
            float firstPosCircleRadius = this.FirstDrawCircleRadius;

            int mapWidth = fullGameMap.Width;
            int mapHeight = fullGameMap.Height;
            double pixelsPerCellX = (double)mapWidth / TotalCellsX;
            double pixelsPerCellY = (double)mapHeight / TotalCellsY;

            Graphics graphics = Graphics.FromImage(fullGameMap);
            Font legendFont = new Font(this.LegendFont, LegendFontSize, LegendFontStyle, GraphicsUnit.Pixel);

            GraphicsStringRenderer legend
                = new GraphicsStringRenderer(graphics, legendFont, this.LegendX, this.LegendY);

            legend.PushString(Brushes.White, "Legend: ");
            legend.Flush();
            legend.NextX += 8;
            foreach (CharacterSaves charSaves in savesForChar)
            {
                bool isFirstDraw = true;
                float prevX = 0.0f;
                float prevY = 0.0f;
                Pen linePen = new Pen(charSaves.Brush, this.LineSize);

                foreach (TesSavegame savegame in charSaves.Saves)
                {
                    double worldspaceX;
                    double worldspaceY;

                    if (!GameData.IsInDefaultWorldspace(
                        savegame.Worldspace1.FormID,
                        savegame.Worldspace2.FormID))
                    {
                        // No interior info present.
                        if (InteriorDB == null)
                            continue;

                        var position = InteriorDB.GetInteriorPosition(savegame.Worldspace2.FormID);
                        if (position == null)
                        {
                            // Player was in foreign worldspace or in foreign
                            // worldspace interior, so skip it.
                            continue;
                        }

                        worldspaceX = position.Item1 / CellSize + CellOffsetX;
                        worldspaceY = position.Item2 / CellSize + CellOffsetY;
                    }
                    else
                    {
                        double cellX = savegame.X / CellSize;
                        double cellY = savegame.Y / CellSize;

                        worldspaceX = cellX + CellOffsetX;
                        worldspaceY = cellY + CellOffsetY;
                    }

                    double x = pixelsPerCellX * worldspaceX;
                    double y = (double)mapHeight - (pixelsPerCellY * worldspaceY);

                    if (isFirstDraw)
                    {
                        isFirstDraw = false;
                        graphics.FillEllipse(charSaves.Brush,
                            (float)(x - (firstPosCircleRadius / 2.0d)),
                            (float)(y - (firstPosCircleRadius / 2.0d)),
                            firstPosCircleRadius,
                            firstPosCircleRadius);
                    }
                    else
                    {
                        graphics.FillEllipse(charSaves.Brush,
                            (float)(x - (posCircleRadius / 2.0d)),
                            (float)(y - (posCircleRadius / 2.0d)),
                            posCircleRadius,
                            posCircleRadius);
                        graphics.DrawLine(linePen,
                            (float)x,
                            (float)y,
                            prevX,
                            prevY);
                    }

                    prevX = (float)x;
                    prevY = (float)y;
                }

                // Drawn at least once
                if (!isFirstDraw)
                {
                    legend.PushString(charSaves.Brush, 
                        String.IsNullOrWhiteSpace(charSaves.CharacterName) ? "No Name" : charSaves.CharacterName);
                }
            }

            legend.Flush();
            LegendEndY = legend.NextY + 10;
            return graphics;
        }
    }
}
