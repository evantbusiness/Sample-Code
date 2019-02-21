using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace Thivierge_Final
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class FinalGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Graphics
        SpriteManager spriteManager;
        Texture2D title;
        Texture2D gameOver;
        Texture2D background;
        Texture2D life;
        Texture2D power;
        Texture2D[] combo;

        //Music
        SoundEffect music;
        SoundEffectInstance musicInstance;

        //Text
        SpriteFont scoreFont;

        //Stats
        public int score;
        public int lives;
        public int consecutivePoints;
        public bool comboState;
        public int level;
        private int levelTimer;
        private int maxLevel;
        public int powerTimer;
        private int powerDisplayTimer;

        //Input
        KeyboardState prevKey;
        KeyboardState currKey;

        //State
        public enum GameState { TITLE, PLAYING, END };
        public GameState currentState = GameState.TITLE;

        public FinalGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 920;
            graphics.PreferredBackBufferHeight = 720;
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
            level = 0;

            spriteManager = new SpriteManager(this);
            Components.Add(spriteManager);

            score = 0;
            lives = 0;
            consecutivePoints = 0;
            comboState = false;
            maxLevel = 4;
            levelTimer = 0;
            powerTimer = 0;
            powerDisplayTimer = 0;

            SoundManager.Initialize(Content);

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

            title = Content.Load<Texture2D>(@"Images\Title");
            gameOver = Content.Load<Texture2D>(@"Images\GameOver");
            background = Content.Load<Texture2D>(@"Images\SpaceBackground");
            life = Content.Load<Texture2D>(@"Images\Life");

            power = Content.Load<Texture2D>(@"Images\Power");

            combo = new Texture2D[7];
            combo[0] = Content.Load<Texture2D>(@"Images\Combo0");
            combo[1] = Content.Load<Texture2D>(@"Images\Combo1");
            combo[2] = Content.Load<Texture2D>(@"Images\Combo2");
            combo[3] = Content.Load<Texture2D>(@"Images\Combo3");
            combo[4] = Content.Load<Texture2D>(@"Images\Combo4");
            combo[5] = Content.Load<Texture2D>(@"Images\Combo5");
            combo[6] = Content.Load<Texture2D>(@"Images\Combo6");

            //Gamma Knife (8-bit)
            music = Content.Load<SoundEffect>(@"Audio\Music");

            scoreFont = Content.Load<SpriteFont>(@"Font\ScoreFont");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevKey = currKey;
            currKey = Keyboard.GetState();

            switch(currentState)
            {
                case GameState.TITLE:
                    if (currKey.IsKeyDown(Keys.Enter) && !prevKey.IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                    {
                        score = 0;
                        lives = 5;
                        consecutivePoints = 0;
                        comboState = false;
                        level = 0;
                        levelTimer = 0;
                        powerTimer = 0;
                        powerDisplayTimer = 0;
                        musicInstance = music.CreateInstance();
                        musicInstance.IsLooped = true;
                        musicInstance.Play();
                        currentState = GameState.PLAYING;
                    }
                    break;
                case GameState.PLAYING:
                    levelTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (levelTimer >= 40000)
                    {
                        if (level < maxLevel)
                        {
                            level++;
                            SoundManager.PlayLevel();
                        }

                        levelTimer = 0;
                    }

                    if (powerTimer > 0)
                    {
                        powerTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                        powerDisplayTimer = powerTimer / 1000;
                    }

                    if (consecutivePoints == 6)
                    {
                        comboState = true;
                    }
                    else
                    {
                        comboState = false;
                    }

                    if (lives <= 0)
                    {
                        currentState = GameState.END;
                        if (musicInstance != null)
                        {
                            musicInstance.Stop();
                        }
                    }
                    break;
                case GameState.END:
                    levelTimer = 0;
                    powerTimer = 0;
                    powerDisplayTimer = 0;

                    if (currKey.IsKeyDown(Keys.Enter) && !prevKey.IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                        currentState = GameState.TITLE;
                    break;
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

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            spriteBatch.DrawString(scoreFont, "Score", new Vector2(670, 75), Color.White);
            spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(670, 100), Color.White);

            spriteBatch.Draw(combo[consecutivePoints], new Vector2(690, 170), Color.White);

            spriteBatch.Draw(power, new Vector2(670, 330), Color.White);

            if (powerDisplayTimer >= 10)
            {
                spriteBatch.DrawString(scoreFont, powerDisplayTimer.ToString(), new Vector2(824, 380), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.2f);
            } else
            {
                spriteBatch.DrawString(scoreFont, "0" + powerDisplayTimer.ToString(), new Vector2(824, 380), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.2f);
            }

            spriteBatch.Draw(life, new Vector2(685, 470), Color.White);
            spriteBatch.DrawString(scoreFont, "X" + lives.ToString(), new Vector2(720, 475), Color.White);

            spriteBatch.DrawString(scoreFont, "Level " + (level + 1).ToString(), new Vector2(700, 650), Color.White);

            if (currentState == GameState.TITLE)
            {
                spriteBatch.Draw(title, Vector2.Zero, null, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
            }

            if (currentState == GameState.END)
            {
                spriteBatch.Draw(gameOver, Vector2.Zero, null, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
