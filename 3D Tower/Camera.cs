using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace Thivierge_Midterm
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        //Camera vectors
        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraDirectionNoY;
        Vector3 cameraUp;

        //Speed
        float speed = 0.3f;

        //Jetpack
        public int jetPackPower = 5000; //in milliseconds
        public float movingForce = 0.15f;
        public bool flying = false;
        public bool descending = false;
        public bool clipping;

        //Collision
        public BoundingSphere bottomSphere;
        public BoundingSphere midSphere;
        public BoundingSphere topSphere;
        float Y_Adjust = 2.5f;

        //Mouse state
        MouseState prevMouseState;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            //Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            bottomSphere = new BoundingSphere(cameraPosition, 0.5f);
            midSphere = new BoundingSphere(cameraPosition, 0.05f);
            topSphere = new BoundingSphere(cameraPosition, 0.5f);

            clipping = false;

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 8000);
        }

        public override void Initialize()
        {
            //Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
			//Create vector which ignores y to move forward and backward without flying
            cameraDirectionNoY = new Vector3(cameraDirection.X, 0, cameraDirection.Z);
            cameraDirectionNoY.Normalize();

            if(((PlatformerGame)Game).currentGameState == PlatformerGame.GameState.PLAY)
            {
                KeyboardState ks = Keyboard.GetState();

                if(!clipping)
                {
                    //Move forward/backward
                    if (ks.IsKeyDown(Keys.W))
                    {
                        cameraPosition += cameraDirectionNoY * speed;
                        bottomSphere.Center = cameraPosition;
                        midSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + (Y_Adjust / 2), cameraPosition.Z);
                        topSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + Y_Adjust, cameraPosition.Z);
                    }
                    if (ks.IsKeyDown(Keys.S))
                    {
                        cameraPosition -= cameraDirectionNoY * speed;
                        bottomSphere.Center = cameraPosition;
                        midSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + (Y_Adjust / 2), cameraPosition.Z);
                        topSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + Y_Adjust, cameraPosition.Z);
                    }


                    //Move side to side
                    if (ks.IsKeyDown(Keys.A))
                    {
                        cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
                        bottomSphere.Center = cameraPosition;
                        midSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + (Y_Adjust / 2), cameraPosition.Z);
                        topSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + Y_Adjust, cameraPosition.Z);
                    }
                    if (ks.IsKeyDown(Keys.D))
                    {
                        cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;
                        bottomSphere.Center = cameraPosition;
                        midSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + (Y_Adjust / 2), cameraPosition.Z);
                        topSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + Y_Adjust, cameraPosition.Z);
                    }
                }

                //Use jetpack
                if(ks.IsKeyDown(Keys.Space))
                {
                    if(!flying && jetPackPower > 0)
                    {
                        descending = false;
                        flying = true;
                    }
                } else
                {
                    flying = false;
                    if(cameraPosition.Y > 4)
                    {
                        descending = true;
                    } else
                    {
                        descending = false;
                        UpdateEnergy(gameTime, true);
                    }
                }

                if(flying)
                {
                    cameraPosition += new Vector3(0, movingForce, 0);
                    UpdateEnergy(gameTime, false);

                    if(jetPackPower <= 0)
                    {
                        flying = false;
                        descending = true;
                    }

                    bottomSphere.Center = cameraPosition;
                    midSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + (Y_Adjust / 2), cameraPosition.Z);
                    topSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + Y_Adjust, cameraPosition.Z);
                }
                if (descending)
                {
                    cameraPosition -= new Vector3(0, movingForce, 0);

                    bottomSphere.Center = cameraPosition;
                    midSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + (Y_Adjust / 2), cameraPosition.Z);
                    topSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + Y_Adjust, cameraPosition.Z);
                }


                //Yaw rotation
                cameraDirection = Vector3.Transform(cameraDirection,
                    Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 150) *
                    (Mouse.GetState().X - prevMouseState.X)));

                //Pitch rotation
                cameraDirection = Vector3.Transform(cameraDirection,
                    Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
                    (MathHelper.PiOver4 / 100) *
                    (Mouse.GetState().Y - prevMouseState.Y)));


                //Reset prevMouseState
                prevMouseState = Mouse.GetState();

                //Recreate the camera view matrix
                CreateLookAt();
            }
            
            
            base.Update(gameTime);
        }

        public void UpdateEnergy(GameTime gameTime, bool increase)
        {
            if(jetPackPower > 0 && !increase)
            {
                jetPackPower -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (jetPackPower < 5000 && increase)
            {
                jetPackPower += gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        public void posAdjust(Vector3 adjustVector)
        {
            cameraPosition += adjustVector;
            bottomSphere.Center = cameraPosition;
            midSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + (Y_Adjust / 2), cameraPosition.Z);
            topSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + Y_Adjust, cameraPosition.Z);
        }

        public void posControl(Vector3 controlVector)
        {
            cameraPosition = controlVector;
            bottomSphere.Center = cameraPosition;
            midSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + (Y_Adjust / 2), cameraPosition.Z);
            topSphere.Center = new Vector3(cameraPosition.X, cameraPosition.Y + Y_Adjust, cameraPosition.Z);
        }

        public void CheckWin(BoundingSphere levelEnd)
        {
            if(bottomSphere.Intersects(levelEnd))
            {
                ((PlatformerGame)Game).ChangeGameState(PlatformerGame.GameState.WIN);
            }
        }

        public void setSpeed(float speed)
        {
            this.speed = speed;
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                cameraPosition + cameraDirection, cameraUp);
        }
    }
}
