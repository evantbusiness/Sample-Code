using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Thivierge_Final
{
    class Missile : Sprite
    {
        public int scoreValue;
        public bool remove;
        public bool outOfBounds;

        public override Vector2 direction
        {
            get { return speed; }
        }

        //Constructor
        public Missile(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            int scoreValue)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed)
        {
            this.scoreValue = scoreValue;
        }

        public Vector2 GetLocation()
        {
            return this.position;
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //Movement
            position += direction;

            if (position.Y > clientBounds.Height + 10)
            {
                remove = true;
                outOfBounds = true;
            }

            base.Update(gameTime, clientBounds);
        }
    }
}
