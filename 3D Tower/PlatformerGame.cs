using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Thivierge_Midterm
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PlatformerGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Models manager
        ModelManager modelManager;

        //Camera
        public Camera camera { get; protected set; }

        //Primitives
        LevelPrimitives primitives;

        //Game States
        public enum GameState { START, PLAY, INSTRUCTIONS, END, WIN }

        public GameState currentGameState = GameState.START;

        SplashScreen splashScreen;

        //Display
        int time = 120000; //in milliseconds
        int displayTime;
        SpriteFont scoreFont;

        //Jetpack HUD
        Texture2D[] jetPowerMeter;
        Vector2 powerPos = new Vector2(10, 670);

        //Level End
        public Vector3 endPos = new Vector3(0, 279, 0);
        private BoundingSphere levelEnd;

        public PlatformerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Preferred Resolution
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Initialize Camera
            camera = new Camera(this, new Vector3(0, 4, 25),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            //Initialize Level
            primitives = new LevelPrimitives(this, camera);
            Components.Add(primitives);
            primitives.Enabled = false;
            primitives.Visible = false;

            //Initialize ModelManager
            modelManager = new ModelManager(this);
            Components.Add(modelManager);
            modelManager.Enabled = false;
            modelManager.Visible = false;

            //Splash Screen
            splashScreen = new SplashScreen(this);
            Components.Add(splashScreen);
            splashScreen.SetData("Platform Game",
                currentGameState);

            levelEnd = new BoundingSphere(endPos, 4.0f);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            jetPowerMeter = new Texture2D[6];
            jetPowerMeter[0] = Content.Load<Texture2D>(@"Textures\jetpack1");
            jetPowerMeter[1] = Content.Load<Texture2D>(@"textures\jetpack2");
            jetPowerMeter[2] = Content.Load<Texture2D>(@"textures\jetpack3");
            jetPowerMeter[3] = Content.Load<Texture2D>(@"textures\jetpack4");
            jetPowerMeter[4] = Content.Load<Texture2D>(@"textures\jetpack5");
            jetPowerMeter[5] = Content.Load<Texture2D>(@"textures\jetpack6");

            scoreFont = Content.Load<SpriteFont>(@"fonts\ScoreFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            if (currentGameState == GameState.PLAY)
            {
                UpdateTime(gameTime);

                SimpleCollision();

                camera.CheckWin(levelEnd);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);

            if (currentGameState == GameState.PLAY)
            {
                spriteBatch.Begin();

                //Draw the current score
                string scoreText = "Time Left:" + displayTime;
                spriteBatch.DrawString(scoreFont, scoreText,
                    new Vector2(10, 20), Color.White);

                if (camera.jetPackPower >= 5000)
                {
                    spriteBatch.Draw(jetPowerMeter[0], powerPos, Color.White);
                }
                if (camera.jetPackPower <= 5000 && camera.jetPackPower >= 4000)
                {
                    spriteBatch.Draw(jetPowerMeter[0], powerPos, Color.White);
                }
                if (camera.jetPackPower <= 4000 && camera.jetPackPower >= 3000)
                {
                    spriteBatch.Draw(jetPowerMeter[1], powerPos, Color.White);
                }
                if (camera.jetPackPower <= 3000 && camera.jetPackPower >= 2000)
                {
                    spriteBatch.Draw(jetPowerMeter[2], powerPos, Color.White);
                }
                if (camera.jetPackPower <= 2000 && camera.jetPackPower >= 1000)
                {
                    spriteBatch.Draw(jetPowerMeter[3], powerPos, Color.White);
                }
                if (camera.jetPackPower <= 1000 && camera.jetPackPower >= 0)
                {
                    spriteBatch.Draw(jetPowerMeter[4], powerPos, Color.White);
                }
                if (camera.jetPackPower <= 0)
                {
                    spriteBatch.Draw(jetPowerMeter[5], powerPos, Color.White);
                }
                    spriteBatch.End();
            }
        }

        public void UpdateTime(GameTime gameTime)
        {
            if (time > 0)
            {
                time -= gameTime.ElapsedGameTime.Milliseconds;
                displayTime = time / 1000;
            }
            else
            {
                ChangeGameState(GameState.END);
            }
        }

        public void SimpleCollision()
        {
            if (camera.cameraPosition.Z < -28)
            {
                camera.posControl(new Vector3(camera.cameraPosition.X, camera.cameraPosition.Y, -28));
            }

            if (camera.cameraPosition.Z > 28)
            {
                camera.posControl(new Vector3(camera.cameraPosition.X, camera.cameraPosition.Y, 28));
            }

            if (camera.cameraPosition.X < -28)
            {
                camera.posControl(new Vector3(-28, camera.cameraPosition.Y, camera.cameraPosition.Z));
            }

            if (camera.cameraPosition.X > 28)
            {
                camera.posControl(new Vector3(28, camera.cameraPosition.Y, camera.cameraPosition.Z));
            }

            if (camera.cameraPosition.Y > 298)
            {
                camera.posControl(new Vector3(camera.cameraPosition.X, 298, camera.cameraPosition.Z));
            }

            if (camera.cameraPosition.Y < 4)
            {
                camera.posControl(new Vector3(camera.cameraPosition.X, 4, camera.cameraPosition.Z));
            }
        }

        public void ChangeGameState(GameState state)
        {
            currentGameState = state;

            switch (currentGameState)
            {
                case GameState.INSTRUCTIONS:
                    splashScreen.SetData("Instructions",
                        GameState.INSTRUCTIONS);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    primitives.Enabled = false;
                    primitives.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;

                    break;

                case GameState.PLAY:
                    modelManager.Enabled = true;
                    modelManager.Visible = true;
                    primitives.Enabled = true;
                    primitives.Visible = true;
                    splashScreen.Enabled = false;
                    splashScreen.Visible = false;

                    break;

                case GameState.END:
                    splashScreen.SetData("Game Over.", GameState.END);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    primitives.Enabled = false;
                    primitives.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;

                    break;

                case GameState.WIN:
                    int finalTime = displayTime;
                    splashScreen.SetData("You Win!" +
                        "\nScore:" + finalTime, GameState.WIN);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    primitives.Enabled = false;
                    primitives.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;
                    break;
            }
        }
    }
}
