using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesSaveLocationTracker.Tes.Skyrim;

namespace TesSaveLocationTracker.Renderer
{
    public struct CharacterSaves
    {
        public IEnumerable<SkyrimSavegame> Saves { get; set; }

        public Brush Brush { get; set; }

        public string CharacterName { get; set; }
    }

    public class SkyrimSavesRenderer
    {
        public List<Brush> DrawBrushes { get; protected set; }

        public float FirstDrawCircleRadius { get; set; }

        public float DrawCircleRadius { get; set; }

        public float LegendX { get; set; }

        public float LegendY { get; set; }

        public float LegendEndY { get; private set; }

        public int LegendFontSize { get; set; }

        public SkyrimSavesRenderer(List<Brush> brushes)
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

        public Graphics Render(Image fullSkyrimMap, IEnumerable<SkyrimSavegame> saves)
        {
            var groupedSaves = saves.GroupBy((save) => save.CharacterName.Normalize());

            // see index++
            int index = 0;
            IEnumerable<CharacterSaves> savesForChar = groupedSaves.Select((charSaves) =>
            {
                return new CharacterSaves()
                {
                    Saves = charSaves.OrderBy((sg) => sg.SaveNumber),
                    CharacterName = saves.First().CharacterName,
                    Brush = GetBrushByIndex(index++)
                };
            });

            float posCircleRadius = this.DrawCircleRadius;
            float firstPosCircleRadius = this.FirstDrawCircleRadius;

            const double cellSize = 4096.0d; // skyrim cell size
            const int cellOffsetX = 74; // skyrim cell X starting from -74
            const int cellOffsetY = 50; // from -50
            const int cellsX = 74 + 75; // total cells X
            const int cellsY = 50 + 49; // total cells Y

            int mapWidth = fullSkyrimMap.Width;
            int mapHeight = fullSkyrimMap.Height;
            double pixelsPerCellX = (double)mapWidth / cellsX;
            double pixelsPerCellY = (double)mapHeight / cellsY;

            Graphics graphics = Graphics.FromImage(fullSkyrimMap);
            Font legendFont = new Font("Segoe UI", LegendFontSize, FontStyle.Regular, GraphicsUnit.Pixel);

            GraphicsStringRenderer legend
                = new GraphicsStringRenderer(graphics, legendFont, this.LegendX, this.LegendY);

            legend.DrawString(Brushes.White, "Legend: ");
            legend.NextX += 8;
            foreach (CharacterSaves charSaves in savesForChar)
            {
                legend.DrawString(charSaves.Brush, charSaves.CharacterName);
                bool isFirstDraw = true;
                float prevX = 0.0f;
                float prevY = 0.0f;
                Pen linePen = new Pen(charSaves.Brush, 2f);

                foreach (SkyrimSavegame savegame in charSaves.Saves)
                {
                    double cellX = savegame.X / cellSize;
                    double cellY = savegame.Y / cellSize;

                    double x = pixelsPerCellX * (cellX + cellOffsetX);
                    double y = (double)mapHeight - pixelsPerCellY * (cellY + cellOffsetY);

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
