using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Thivierge_FinalGA
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class RobotArtist : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Display
        private Texture2D display;
        private Texture2D select;
        private Texture2D draw;
        private Texture2D complete;
        private Grid canvas;

        // Font
        SpriteFont scoreFont;

        // Options
        private int maxNumOptions = 11;
        private int currentSelection;

        private string[] optionText;

        //Keyboard
        KeyboardState prevKS;
        KeyboardState currKS;

        //Target container
        Texture2D targetImage;

        // States
        public enum appState { SELECT, DRAW, COMPLETE }
        public appState currentState = appState.SELECT; 

        public RobotArtist()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 980;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            display = Content.Load<Texture2D>(@"Display\Display");
            select = Content.Load<Texture2D>(@"Display\Select");
            draw = Content.Load<Texture2D>(@"Display\Draw");
            complete = Content.Load<Texture2D>(@"Display\Complete");

            canvas = new Grid(
                Content.Load<Texture2D>(@"Display\Grid"),
                Content.Load<Texture2D>(@"Display\DrawMap"),
                Content.Load<Texture2D>(@"Display\Unit"),
                new Vector2(81, 70));

            scoreFont = Content.Load<SpriteFont>(@"Font\ScoreFont");

            currentSelection = 0;

            optionText = new string[maxNumOptions];

            optionText[0] = "0. Quit the application";
            optionText[1] = "1. Chemical Flask";
            optionText[2] = "2. Coffee Cup";
            optionText[3] = "3. Video Game Controller";
            optionText[4] = "4. Power Cord";
            optionText[5] = "5. Floppy Disk";
            optionText[6] = "6. Nintendo Gameboy";
            optionText[7] = "7. Hourglass";
            optionText[8] = "8. Magnet";
            optionText[9] = "9. Super Mushroom";
            optionText[10] = "10. Rotary Phone";

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
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevKS = currKS;
            currKS = Keyboard.GetState();

            switch (currentState)
            {
                case appState.SELECT:
                    GA.generation = 0;
                    if(currKS.IsKeyDown(Keys.Space))
                    {
                        SelectOption(currentSelection);
                        if(currentSelection != 0)
                        {
                            GA.InitPopulation();
                            GA.SetTarget(ImageData.ScanImage(targetImage));
                            currentState = appState.DRAW;
                        }
                    }
                    else if(currKS.IsKeyDown(Keys.Down))
                    {
                        if(prevKS.IsKeyUp(Keys.Down))
                        {
                            currentSelection++;
                            currentSelection = currentSelection % maxNumOptions;
                        }
                    }
                    else if(currKS.IsKeyDown(Keys.Up))
                    {
                        if(prevKS.IsKeyUp(Keys.Up))
                        {
                            currentSelection--;
                            if (currentSelection < 0)
                                currentSelection = currentSelection + maxNumOptions;
                        }   
                    }
                    break;

                case appState.DRAW:
                    if(!GA.complete)
                    {
                        GA.Update();
                    }
                    else
                    {
                        currentState = appState.COMPLETE;
                    }
                    break;

                case appState.COMPLETE:
                    if(currKS.IsKeyDown(Keys.Q))
                    {
                        this.Exit();
                    }
                    else if(prevKS.IsKeyDown(Keys.D))
                    {
                        currentState = appState.SELECT;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        private void SelectOption(int option)
        {
            switch(option)
            {
                case 0:
                    this.Exit();
                    break;

                case 1:
                    targetImage = Content.Load<Texture2D>(@"Data\ChemicalFlask");
                    break;

                case 2:
                    targetImage = Content.Load<Texture2D>(@"Data\Coffee");
                    break;

                case 3:
                    targetImage = Content.Load<Texture2D>(@"Data\Controller");
                    break;

                case 4:
                    targetImage = Content.Load<Texture2D>(@"Data\Cord");
                    break;

                case 5:
                    targetImage = Content.Load<Texture2D>(@"Data\Floppydisk");
                    break;

                case 6:
                    targetImage = Content.Load<Texture2D>(@"Data\Gameboy");
                    break;

                case 7:
                    targetImage = Content.Load<Texture2D>(@"Data\Hourglass");
                    break;

                case 8:
                    targetImage = Content.Load<Texture2D>(@"Data\Magnet");
                    break;

                case 9:
                    targetImage = Content.Load<Texture2D>(@"Data\Mushroom");
                    break;

                case 10:
                    targetImage = Content.Load<Texture2D>(@"Data\Phone");
                    break;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(display, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(scoreFont, "Gen:" + GA.generation.ToString(), new Vector2(60, 15), Color.White);

            canvas.Draw(spriteBatch);

            if (currentState != appState.SELECT)
            {
                canvas.DrawOntoGrid(spriteBatch, GA.GetBest());
            }

            if(currentState == appState.SELECT)
            {
                spriteBatch.Draw(select, new Vector2(540, 130), Color.White);
                spriteBatch.DrawString(scoreFont, optionText[currentSelection], new Vector2(67, 517), Color.White);
            }
            else if(currentState == appState.DRAW)
            {
                spriteBatch.Draw(draw, new Vector2(540, 130), Color.White);
                spriteBatch.DrawString(scoreFont, optionText[currentSelection], new Vector2(67, 517), Color.White);
            }
            else if(currentState == appState.COMPLETE)
            {
                spriteBatch.Draw(complete, new Vector2(540, 130), Color.White);
                spriteBatch.DrawString(scoreFont, "Press Q to Quit or D to draw a new image", new Vector2(67, 517), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
