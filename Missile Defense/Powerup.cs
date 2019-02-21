using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Thivierge_Final
{
    class Powerup : Sprite
    {
        public int powerID;
        public bool remove;

        public override Vector2 direction
        {
            get { return speed; }
        }

        //Constructor
        public Powerup(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            int powerID)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed)
        {
            this.powerID = powerID;
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            position += direction;

            if (position.Y > clientBounds.Height + 10)
            {
                remove = true;
            }

            base.Update(gameTime, clientBounds);
        }
    }
}
