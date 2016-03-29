using System;
using System.Collections;
using System.Collections.Generic;
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

        public Queue<Tuple<Brush, string>> strings;

        public GraphicsStringRenderer(Graphics graphics, Font font, float x, float y)
        {
            this.Graphics = graphics;
            this.Font = font;

            if (font.Unit != GraphicsUnit.Pixel)
                throw new NotSupportedException("Font units other than pixels are not supported.");

            StepSize = font.Size + 3;
            NextX = x;
            NextY = y;

            strings = new Queue<Tuple<Brush, string>>();
        }

        public void PushString(Brush brush, string value)
        {
            strings.Enqueue(new Tuple<Brush, string>(brush, value));

        }

        public void Flush()
        {
            while (strings.Count > 0)
            {
                var stringInfo = strings.Dequeue();

                // shadow
                Graphics.DrawString(stringInfo.Item2, Font, Brushes.Black, NextX - 1, NextY - 1);
                Graphics.DrawString(stringInfo.Item2, Font, Brushes.Black, NextX + 1, NextY + 1);
                // actual string
                Graphics.DrawString(stringInfo.Item2, Font, stringInfo.Item1, NextX, NextY);

                NextY += StepSize;
            }
        }
    }
}
