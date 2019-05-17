using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Draw
{
    class MyCanvas
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BackColor { get; private set; }


        private Texture2D map;


        public MyCanvas(int width, int height, Color backColor)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = backColor;
            map = new Texture2D(width, height);
            Clear();

        }

        public Sprite GetSprite()
        {
            map.Apply();
            return Sprite.Create(map, new Rect(0, 0, Width, Height), Vector2.zero);
        }

        public void DrawGridAt(int x,int y, Color color)
        {
            map.SetPixel(x, y, color);
        }

        public void DrawGridAt(int x, int y,int width,int height, Color color, float alpha)
        {
            int xStart = x - width / 2;
            int xEnd = x + width / 2;
            int yStart = y - height / 2;
            int yEnd = y + height / 2;
            color[3] = alpha; 

            for(int i = xStart; i < xEnd; ++i)
            {
                for(int j = yStart; j < yEnd; ++j)
                {
                    if (i < 0 || j < 0 || i > this.Width || j > this.Height)
                        continue;

                    AddPixel(i, j, color);
                }
            }
        }

        public void DrawGridAt(int x, int y, int width, int height, Color color, int borderSize, Color borderColor)
        {
            int xStart = x - width / 2;
            int xEnd = x + width / 2;
            int yStart = y - height / 2;
            int yEnd = y + height / 2;

            int xInnerStart = xStart + borderSize;
            int xInnerEnd = xEnd - 1 - borderSize;
            int yInnerStart = yStart + borderSize;
            int yInnerEnd = yEnd - 1 - borderSize;

            for (int i = xStart; i < xEnd; ++i)
            {
                for (int j = yStart; j < yEnd; ++j)
                {
                    if (i < 0 || j < 0 || i > this.Width || j > this.Height)
                        continue;

                    if (i < xInnerStart || i > xInnerEnd || j < yInnerStart || j > yInnerEnd)
                        map.SetPixel(i, j, borderColor);
                    else
                        map.SetPixel(i, j, color);

                }
            }
        }

        public void Clear()
        {
            FillByColor(BackColor);
        }

        public void FillByColor(Color color)
        {
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    map.SetPixel(x, y, color);
                }
            }
        }

        private bool rand = true;
        private void AddPixel(int x,int y,Color color)
        {
            Color back = map.GetPixel(x, y);

            if (back.Equals(BackColor))
                map.SetPixel(x, y, color);

            else if (rand)
            {
                rand = false;
                map.SetPixel(x, y, color);
            }
            else
                rand = true;
        }
    }
}
