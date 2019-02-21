using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Thivierge_Final
{
    class SoundManager
    {
        private static List<SoundEffect> explosions = new List<SoundEffect>();

        private static int explosionCount = 3; 

        private static SoundEffect playerShot;

        private static SoundEffect powerCollect;

        private static SoundEffect levelUp;

        private static Random rand = new Random(); 


        public static void Initialize(ContentManager content)
        {
            try
            {
                playerShot = content.Load<SoundEffect>(@"Audio\Shot");

                powerCollect = content.Load<SoundEffect>(@"Audio\Boop");

                levelUp = content.Load<SoundEffect>(@"Audio\Level");

                for (int x = 1; x <= explosionCount; x++)
                {
                    explosions.Add(
                        content.Load<SoundEffect>(@"Audio\Explosion" +
                            x.ToString()));
                }
            }
            catch
            {
                Debug.Write("SoundManager Initialization Failed");
            }
        }

        public static void PlayExplosion()
        {
            try
            {
                explosions[rand.Next(0, explosionCount)].Play();
            }
            catch
            {
                Debug.Write("PlayExplosion Failed");
            }
        }

        public static void PlayShot()
        {
            try
            {
                playerShot.Play();
            }
            catch
            {
                Debug.Write("PlayPlayerShot Failed");
            }
        }

        public static void PlayCollect()
        {
            try
            {
                powerCollect.Play();
            }
            catch
            {
                Debug.Write("PlayCollect Failed");
            }
        }

        public static void PlayLevel()
        {
            try
            {
                levelUp.Play();
            }
            catch
            {
                Debug.Write("PlayLevel Failed");
            }
        }
    }
}
