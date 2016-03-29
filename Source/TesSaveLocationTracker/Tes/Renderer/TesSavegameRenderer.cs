using System;
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
        public IList<Brush> DrawBrushes { get; protected set; }

        public float FirstDrawCircleRadius { get; set; }

        public float DrawCircleRadius { get; set; }

        public float LegendX { get; set; }

        public float LegendY { get; set; }

        public float LegendEndY { get; private set; }

        public int LegendFontSize { get; set; }

        public int CellOffsetX { get; set; }

        public int CellOffsetY { get; set; }

        public int TotalCellsX { get; set; }

        public int TotalCellsY { get; set; }

        public double CellSize { get; set; }

        public TesSavegameRenderer(IList<Brush> brushes)
        {
            if (brushes == null)
                throw new ArgumentNullException(nameof(brushes));
            this.DrawBrushes = brushes;
            this.LegendFontSize = 16;
            this.LegendX = 1;
            this.LegendY = 1;
            this.FirstDrawCircleRadius = 8.0f;
            this.DrawCircleRadius = 5.0f;
        }

        private Brush GetBrushByIndex(int index)
        {
            if (index >= DrawBrushes.Count)
                return Brushes.LightGray;
            return DrawBrushes[index];
        }

        public Graphics Render(Image fullSkyrimMap, IEnumerable<TesSavegame> saves)
        {
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

            int mapWidth = fullSkyrimMap.Width;
            int mapHeight = fullSkyrimMap.Height;
            double pixelsPerCellX = (double)mapWidth / TotalCellsX;
            double pixelsPerCellY = (double)mapHeight / TotalCellsY;

            Graphics graphics = Graphics.FromImage(fullSkyrimMap);
            Font legendFont = new Font("Segoe UI", LegendFontSize, FontStyle.Regular, GraphicsUnit.Pixel);

            GraphicsStringRenderer legend
                = new GraphicsStringRenderer(graphics, legendFont, this.LegendX, this.LegendY);

            legend.DrawString(Brushes.White, "Legend: ");
            legend.NextX += 8;
            foreach (CharacterSaves charSaves in savesForChar)
            {
                legend.DrawString(charSaves.Brush, String.IsNullOrWhiteSpace(charSaves.CharacterName) ? "No Name" : charSaves.CharacterName);
                bool isFirstDraw = true;
                float prevX = 0.0f;
                float prevY = 0.0f;
                Pen linePen = new Pen(charSaves.Brush, 2f);

                foreach (TesSavegame savegame in charSaves.Saves)
                {
                    double cellX = savegame.X / CellSize;
                    double cellY = savegame.Y / CellSize;

                    double x = pixelsPerCellX * (cellX + CellOffsetX);
                    double y = (double)mapHeight - pixelsPerCellY * (cellY + CellOffsetY);

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
            }

            LegendEndY = legend.NextY + 10;
            return graphics;
        }
    }
}
