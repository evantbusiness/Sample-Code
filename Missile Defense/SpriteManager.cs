using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;


namespace Thivierge_Final
{
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        //A sprite for the player and a list of automated sprites
        Player player;
        List<Missile> missileList = new List<Missile>();

        //Powerups
        List<Powerup> powerups = new List<Powerup>();
        Texture2D[] powerTex;

        //Random
        Random rand;

        //Spawning
        int nextSpawnTime = 0;
        int timeSinceLastSpawn = 0;

        struct MissileVars
        {
            public Texture2D texture;
            public int value;

            public MissileVars(Texture2D texture, int value)
            {
                this.texture = texture;
                this.value = value;
            }
        }

        List<MissileVars> missileVars;

        struct LevelSet
        {
            public int minSpawnTime;
            public int maxSpawnTime;

            public LevelSet(int minSpawnTime, int maxSpawnTime)
            {
                this.minSpawnTime = minSpawnTime;
                this.maxSpawnTime = maxSpawnTime;
            }
        }

        List<LevelSet> levelSet;


        public SpriteManager(Game game)
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
            rand = new Random();

            powerTex = new Texture2D[2];

            powerTex[0] = Game.Content.Load<Texture2D>(@"Images\Spread");
            powerTex[1] = Game.Content.Load<Texture2D>(@"Images\Rapid");

            missileVars = new List<MissileVars>();

            missileVars.Add(new MissileVars(Game.Content.Load<Texture2D>(@"Images\BlueMissile"), 25));
            missileVars.Add(new MissileVars(Game.Content.Load<Texture2D>(@"Images\GreenMissile"), 50));
            missileVars.Add(new MissileVars(Game.Content.Load<Texture2D>(@"Images\YellowMissile"), 75));
            missileVars.Add(new MissileVars(Game.Content.Load<Texture2D>(@"Images\RedMissile"), 100));

            levelSet = new List<LevelSet>();

            levelSet.Add(new LevelSet(1700, 2000));
            levelSet.Add(new LevelSet(1400, 1700));
            levelSet.Add(new LevelSet(1100, 1400));
            levelSet.Add(new LevelSet(800, 1100));
            levelSet.Add(new LevelSet(500, 800));

            SetNextSpawnTime();

            base.Initialize();
        }

        private void SetNextSpawnTime()
        {
            nextSpawnTime = rand.Next(levelSet[((FinalGame)Game).level].minSpawnTime, levelSet[((FinalGame)Game).level].minSpawnTime);
            timeSinceLastSpawn = 0;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            //Load the sprites
            player = new Player(Game.Content.Load<Texture2D>(@"Images/Ship"), new Vector2(50, 650), new Point(32, 35),
                0, new Point(0, 0), new Point(8, 1), new Vector2(8, 8), 60, Game.Content.Load<Texture2D>(@"Images/Shot"));

            //Call the parent-base
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if(Game.IsActive && ((FinalGame)Game).currentState == FinalGame.GameState.PLAYING)
            {
                //Update player
                player.Update(gameTime, Game.Window.ClientBounds);

                //Update sprites
                foreach (Missile missile in missileList)
                {
                    missile.Update(gameTime, Game.Window.ClientBounds);

                    foreach(AdvancedSprite shot in player.shots)
                    {
                        if(shot.IsBoxColliding(missile.collisionRect))
                        {
                            SoundManager.PlayExplosion();

                            if (!((FinalGame)Game).comboState)
                            {
                                ((FinalGame)Game).consecutivePoints++;
                                ((FinalGame)Game).score += missile.scoreValue;
                            }
                            else
                            {
                                ((FinalGame)Game).score += (missile.scoreValue * 2);
                            }

                            missile.remove = true;
                        }
                    }
                }

                for(int i = 0; i < powerups.Count; i++)
                {
                    powerups[i].Update(gameTime, Game.Window.ClientBounds);

                    if (powerups[i].collisionRect.Intersects(player.collisionRect))
                    {
                        SoundManager.PlayCollect();

                        ((FinalGame)Game).powerTimer = 20000;
                        player.SetPower(powerups[i].powerID);

                        powerups.RemoveAt(i);
                    }
                    else if (powerups[i].remove)
                        powerups.RemoveAt(i);
                }

                //Removal of flagged missiles
                for(int i = 0; i < missileList.Count; i++)
                {
                    if(missileList[i].outOfBounds)
                    {
                        ((FinalGame)Game).lives--;
                        ((FinalGame)Game).consecutivePoints = 0;
                    }
                    if (missileList[i].remove)
                    {
                        int powerChance = rand.Next(0, 15);
                        int powerType = rand.Next(0, 2);

                        if (powerChance == 10)
                            powerups.Add(new Powerup(powerTex[powerType], missileList[i].GetLocation() + new Vector2(-25, 0), new Point(80, 80),
                                        5, new Point(0, 0), new Point(1, 1), new Vector2(0, 4), powerType));

                        missileList.RemoveAt(i);
                    }
                }

                if(((FinalGame)Game).powerTimer <= 0)
                {
                    player.SetPower(2);
                }

                CheckToSpawnMissile(gameTime);
            }

            if(((FinalGame)Game).currentState == FinalGame.GameState.END)
            {
                missileList.Clear();
                player.ClearShots();
            }

            base.Update(gameTime);
        }

        private void CheckToSpawnMissile(GameTime gameTime)
        {
            timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;

            if(timeSinceLastSpawn > nextSpawnTime)
            {
                int randMissile = rand.Next(0, 4);

                missileList.Add(new Missile(missileVars[randMissile].texture, new Vector2(rand.Next(5, 620), -100), new Point(30, 90),
                    5, new Point(0, 0), new Point(2, 1), new Vector2(0, 4), missileVars[randMissile].value));

                SetNextSpawnTime();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            //Draw the player
            if(((FinalGame)Game).currentState == FinalGame.GameState.PLAYING)
            {
                player.Draw(gameTime, spriteBatch);
                player.DrawShots(spriteBatch);
            }

            //Draw all sprites
            foreach (Sprite s in missileList)
                s.Draw(gameTime, spriteBatch);

            foreach (Sprite p in powerups)
                p.Draw(gameTime, spriteBatch);

            if (((FinalGame)Game).powerTimer > 0)
                spriteBatch.Draw(powerTex[player.GetPower()], new Vector2(690, 358), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
