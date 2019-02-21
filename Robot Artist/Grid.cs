using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Thivierge_FinalGA
{
    public class Grid
    {
        //Fields
        private Texture2D display;
        private Texture2D infoPoints;
        private Texture2D unit;
        private Vector2 pos;
        private int pointWidth = 15;
        private int pointHeight = 15;
        public Vector2[,] points;

        public Grid(Texture2D display, Texture2D infoPoints, Texture2D unit, Vector2 pos)
        {
            this.display = display;
            this.infoPoints = infoPoints;
            this.unit = unit;
            this.pos = pos;

            ScanPoints();
        }

        private void ScanPoints()
        {
            points = new Vector2[pointWidth, pointHeight];
            List<Vector2> collection = new List<Vector2>();

            Color pointColor = new Color(new Vector4(255, 0, 0, 0));

            Color[] infoData = new Color[infoPoints.Width * infoPoints.Height];
            infoPoints.GetData(infoData);

            int widthCount = 0;
            int heightCount = 0;

            for (int x = 0; x < infoPoints.Width; x++)
            {
                for (int y = 0; y < infoPoints.Height; y++)
                {
                    byte RColor = infoData[x + y * infoPoints.Width].R;
                    byte GColor = infoData[x + y * infoPoints.Width].G;
                    byte BColor = infoData[x + y * infoPoints.Width].B;
                    if (RColor == pointColor.R && GColor == pointColor.G && BColor == pointColor.B)
                    {
                        if (heightCount < 15)
                        {
                            points[widthCount, heightCount] = new Vector2(x + pos.X, y + pos.Y);
                            heightCount++;
                        }
                        else
                        {
                            heightCount = 0;
                            widthCount++;
                            points[widthCount, heightCount] = new Vector2(x + pos.X, y + pos.Y);
                            heightCount++;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(display, pos, Color.White);
        }

        public void DrawOntoGrid(SpriteBatch spriteBatch, bool[,] drawingPattern)
        {
            for(int x = 0; x < pointWidth; x++)
            {
                for(int y = 0; y < pointHeight; y++)
                {
                    if(drawingPattern[x,y] == true)
                    {
                        spriteBatch.Draw(unit, points[x,y], null, null, new Vector2(unit.Width / 2, unit.Height / 2), 0.0f, null, Color.White, SpriteEffects.None, 0.0f);
                    }
                }
            }
        }
    }
}
