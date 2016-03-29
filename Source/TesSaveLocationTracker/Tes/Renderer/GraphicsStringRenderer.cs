using System;
using System.Drawing;

namespace TesSaveLocationTracker.Tes.Renderer
{
    public class GraphicsStringRenderer
    {
        public Graphics Graphics { get; protected set; }

        public Font Font { get; protected set; }

        public float NextX { get; set; }

        public float NextY { get; set; }

        public float StepSize { get; protected set; }

        public GraphicsStringRenderer(Graphics graphics, Font font, float x, float y)
        {
            this.Graphics = graphics;
            this.Font = font;

            if (font.Unit != GraphicsUnit.Pixel)
                throw new NotSupportedException("Font units other than pixels are not supported.");

            StepSize = font.Size + 3;
            NextX = x;
            NextY = y;
        }

        public void DrawString(Brush brush, string value)
        {
            // shadow
            Graphics.DrawString(value, Font, Brushes.Black, NextX + 1, NextY + 1);
            // actual string
            Graphics.DrawString(value, Font, brush, NextX, NextY);

            NextY += StepSize;
        }
    }
}
