using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Thivierge_FinalGA
{
    static class ImageData
    {
        public static bool[,] ScanImage(Texture2D image)
        {
            bool[,] imageResults = new bool[image.Width, image.Height];

            Color[] imageData = new Color[image.Width * image.Height];
            image.GetData(imageData);

            for(int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    byte color = imageData[x + y * image.Width].R;
                    if(color == Color.Black.R)
                    {
                        imageResults[x, y] = true;
                    } 
                    else
                    {
                        imageResults[x, y] = false;
                    }
                }
            }
           
            return imageResults;
        }
    }
}
