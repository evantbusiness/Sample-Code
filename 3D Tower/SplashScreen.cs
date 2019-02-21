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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SplashScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //Text strings
        string textToDraw;
        string secondaryTextToDraw;

        //Fonts and drawing
        SpriteFont spriteFont;
        SpriteFont secondarySpriteFont;
        SpriteBatch spriteBatch;
        bool isTitle;
        Texture2D background;

        //Game state
        PlatformerGame.GameState currentGameState;

        public SplashScreen(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Load fonts
            spriteFont = Game.Content.Load<SpriteFont>(@"fonts\SplashScreenFontLarge");
            secondarySpriteFont = Game.Content.Load<SpriteFont>(@"fonts\SplashScreenFont");

            //bg image
            background = Game.Content.Load<Texture2D>(@"Textures\background");

            //Create sprite batch
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //Did the player hit Enter?
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                //If we're not in end game, move to play state
                if (currentGameState == PlatformerGame.GameState.INSTRUCTIONS ||
                    currentGameState == PlatformerGame.GameState.START)
                {
                    isTitle = false;
                    ((PlatformerGame)Game).ChangeGameState(PlatformerGame.GameState.PLAY);
                }

                //If we are in end game, exit
                else if (currentGameState == PlatformerGame.GameState.END ||
                        currentGameState == PlatformerGame.GameState.WIN)
                    Game.Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.I))
            {
                if (currentGameState == PlatformerGame.GameState.START)
                {
                    ((PlatformerGame)Game).ChangeGameState(PlatformerGame.GameState.INSTRUCTIONS);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if(isTitle)
            {
                spriteBatch.Draw(background,
                    new Vector2((Game.Window.ClientBounds.Width / 2)
                        - (background.Width / 2),
                        (Game.Window.ClientBounds.Height / 2)
                        - (background.Height / 2)), Color.White);
            }

            //Get size of string
            Vector2 TitleSize = spriteFont.MeasureString(textToDraw);

            //Draw main text
            spriteBatch.DrawString(spriteFont, textToDraw,
                new Vector2(Game.Window.ClientBounds.Width / 2
                    - TitleSize.X / 2,
                    Game.Window.ClientBounds.Height / 2),
                    Color.White);

            //Draw subtext
            spriteBatch.DrawString(secondarySpriteFont,
                secondaryTextToDraw,
                new Vector2(Game.Window.ClientBounds.Width / 2
                    - secondarySpriteFont.MeasureString(
                        secondaryTextToDraw).X / 2,
                    Game.Window.ClientBounds.Height / 2 +
                    TitleSize.Y + 10),
                    Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetData(string main, PlatformerGame.GameState currGameState)
        {
            textToDraw = main;
            this.currentGameState = currGameState;

            switch (currentGameState)
            {
                case PlatformerGame.GameState.START:
                    isTitle = true;
                    secondaryTextToDraw = "Press ENTER to begin, or I for instructions";
                    break;
                case PlatformerGame.GameState.INSTRUCTIONS:
                    isTitle = true;
                    secondaryTextToDraw = " Use WASD keys to move and space to use your jetpack! \n    Reach the top of the tower before time is up. \n\n                 Press ENTER to begin";
                    break;
                case PlatformerGame.GameState.END:
                    isTitle = false;
                    secondaryTextToDraw = "Press ENTER to quit";
                    break;
                case PlatformerGame.GameState.WIN:
                    isTitle = false;
                    secondaryTextToDraw = "Press ENTER to quit";
                    break;
            }
        }

    }
}
