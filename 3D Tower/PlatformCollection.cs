using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Thivierge_Midterm
{
    class PlatformCollection
    {
        List<Platform> platforms = new List<Platform>();

        public PlatformCollection(Game game)
        {
            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(0, 10, -20)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(8, 23, 0)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-12, 39, 0)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-3, 56, 9)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(12, 71, -6)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-8, 78, -18)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(25, 94, -30)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(25, 112, -22.2f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(25, 112, -14.4f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-3, 132, -4)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-25, 157, 22)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-17.2f, 157, 22)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(2, 180, 22)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-15, 194, -10)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(14, 212, -10)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(25, 230, -11.8f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(25, 230, -4)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(25, 230, 3.8f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(0, 245, -4)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(7.8f, 260, -4)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(7.8f, 260, 3.8f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(0, 260, 3.8f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-7.8f, 260, 3.8f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-7.8f, 260, -4)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(-7.8f, 260, -11.8f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(0, 260, -11.8f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(7.8f, 260, -11.8f)));

            platforms.Add(new Platform(
                game.Content.Load<Model>(@"Models\platform"), new Vector3(0, 275, -4)));
        }

        public List<Platform> GetPlatforms()
        {
            return platforms;
        }
    }
}
