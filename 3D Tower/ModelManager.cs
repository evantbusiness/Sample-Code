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
    public class ModelManager : DrawableGameComponent
    {
        //List of platforms
        List<Platform> platforms = new List<Platform>();

        //List of other models
        List<BasicModel> models = new List<BasicModel>();

        public ModelManager(Game game)
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
            //Add models to list
            PlatformCollection platformList = new PlatformCollection(Game);
            platforms = platformList.GetPlatforms();

            Vector3 trophyPos = ((PlatformerGame)Game).endPos;
            trophyPos.Y = ((PlatformerGame)Game).endPos.Y - 3.5f;

            models.Add(new Trophy(
                Game.Content.Load<Model>(@"Models\Trophy"), trophyPos));

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            UpdatePlatforms();

            UpdateModels();

            PlatformCollision(gameTime);

            base.Update(gameTime);
        }

        protected void UpdatePlatforms()
        {
            //Loop through all models and call Update
            for (int i = 0; i < platforms.Count; ++i)
            {
                platforms[i].Update();
            }
        }

        protected void UpdateModels()
        {
            //Loop through all models and call Update
            for (int i = 0; i < models.Count; ++i)
            {
                models[i].Update();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            
            foreach (Platform p in platforms)
            {
                p.Draw(((PlatformerGame)Game).camera);
            }

            //Loop through and draw each model
            foreach (BasicModel bm in models)
            {
                bm.Draw(((PlatformerGame)Game).camera);
            }

            base.Draw(gameTime);
        }

        protected void PlatformCollision(GameTime gameTime)
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                float opposingForce = ((PlatformerGame)Game).camera.movingForce;

                if (platforms[i].CollidesWith(platforms[i].box, ((PlatformerGame)Game).camera.bottomSphere))
                {
                    ((PlatformerGame)Game).camera.posAdjust(new Vector3(0, opposingForce, 0));
                    ((PlatformerGame)Game).camera.UpdateEnergy(gameTime, true);
                }

                if (platforms[i].CollidesWith(platforms[i].box, ((PlatformerGame)Game).camera.topSphere))
                {
                    ((PlatformerGame)Game).camera.posAdjust(new Vector3(0, -opposingForce, 0));
                }

                if (platforms[i].CollidesWith(platforms[i].box, ((PlatformerGame)Game).camera.midSphere))
                {
                    ((PlatformerGame)Game).camera.clipping = true;
                }
                else
                {
                    ((PlatformerGame)Game).camera.clipping = false;
                }
            }
        }
    }
}
