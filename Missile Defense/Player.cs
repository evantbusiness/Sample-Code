using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Thivierge_Final
{
    class Player : Sprite
    {
        //Mouse movement
        MouseState prevMouseState;
        MouseState currMouseState;

        //Shots
        public List<AdvancedSprite> shots = new List<AdvancedSprite>();
        Texture2D shotTexture;
        private float shotTimer = 0.0f;
        private float minShotTimer = 0.4f;
        private Vector2 gunOffset = new Vector2(14, -4);
        private float shotSpeed = 250f;
        private int shotRadius = 2;

        //Shot Type (Powerups)
        private enum ShotType { SPREAD, RAPID, SINGLE };
        private ShotType currentShot = ShotType.SINGLE;

        public override Vector2 direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;

                KeyboardState ks = Keyboard.GetState();
                GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

                if (ks.IsKeyDown(Keys.Left))
                    inputDirection.X -= 1;
                if (ks.IsKeyDown(Keys.Right))
                    inputDirection.X += 1;

                if (gamepadState.ThumbSticks.Left.X != 0)
                    inputDirection.X += gamepadState.ThumbSticks.Left.X;

                return inputDirection * speed;
            }
        }

        //Constructor
        public Player(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, Texture2D shotTexture)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed)
        {
            this.shotTexture = shotTexture;
        }

        //Constrcutor with Framerate
        public Player(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame, Texture2D shotTexture)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame)
        {
            this.shotTexture = shotTexture;
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //Direction based movement
            position += direction;

            //Move if mouse was moved
            currMouseState = Mouse.GetState();
            if (currMouseState.X != prevMouseState.X)
            {
                position = new Vector2(currMouseState.X, position.Y);
            }
            prevMouseState = currMouseState;

            shotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Space) || currMouseState.LeftButton == ButtonState.Pressed)
                FireShot();

            //Keep inside window
            if (position.X < 0)
                position.X = 0;
            if (position.X > 650 - frameSize.X)
                position.X = 650 - frameSize.X;

            UpdateShots(gameTime, new Rectangle(0, 0, 650, 720));

            base.Update(gameTime, clientBounds);
        }

        public int GetPower()
        {
            return (int)currentShot;
        }

        public void SetPower(int power)
        {
            currentShot = (ShotType)power;
        }

        //Fire a shot
        private void FireShot()
        {
            if (shotTimer >= minShotTimer)
            {
                switch (currentShot)
                {
                    case ShotType.SPREAD:
                        minShotTimer = 0.4f;
                        CreateShot(this.position + gunOffset, new Rectangle(0, 0, 5, 5), 1, new Vector2(0, -1));
                        CreateShot(this.position + gunOffset, new Rectangle(0, 0, 5, 5), 1, new Vector2(0.5f, -1));
                        CreateShot(this.position + gunOffset, new Rectangle(0, 0, 5, 5), 1, new Vector2(-0.5f, -1));
                        break;
                    case ShotType.RAPID:
                        minShotTimer = 0.1f;
                        CreateShot(this.position + gunOffset, new Rectangle(0, 0, 5, 5), 1, new Vector2(0, -1));
                        break;
                    case ShotType.SINGLE:
                        minShotTimer = 0.4f;
                        CreateShot(this.position + gunOffset, new Rectangle(0, 0, 5, 5), 1, new Vector2(0, -1));
                        break;
                }

                SoundManager.PlayShot();
                shotTimer = 0.0f;
            }
        }

        //Create a shot
        public void CreateShot(Vector2 location, Rectangle initialFrame, int frameCount, Vector2 velocity)
        {
            AdvancedSprite thisShot = new AdvancedSprite(location, shotTexture, initialFrame, velocity);

            thisShot.Velocity *= shotSpeed;

            for (int x = 1; x < frameCount; x++)
            {
                thisShot.AddFrame(new Rectangle(
                initialFrame.X + (initialFrame.Width * x),
                initialFrame.Y,
                initialFrame.Width,
                initialFrame.Height));
            }

            thisShot.CollisionRadius = shotRadius;
            shots.Add(thisShot);
        }

        //Clear the shots (for game over)
        public void ClearShots()
        {
            shots.Clear();
        }

        //Update Shots
        public void UpdateShots(GameTime gameTime, Rectangle playArea)
        {
            for (int x = shots.Count - 1; x >= 0; x--)
            {
                shots[x].Update(gameTime);

                if (!playArea.Intersects(shots[x].Destination))
                {
                    shots.RemoveAt(x);
                }
            }
        }

        //Draw Shots
        public void DrawShots(SpriteBatch spriteBatch)
        {
            foreach (AdvancedSprite shot in shots)
            {
                shot.Draw(spriteBatch);
            }
        }
    }
}
