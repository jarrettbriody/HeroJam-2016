using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace ZombieGame
{
    class SoundManager
    {

        Dictionary<string, SoundEffect> soundList = new Dictionary<string, SoundEffect>();
        public bool soundActive { get; set; }
        public float Volume { get; set; }

        public SoundManager(SoundEffect[] sounds, string[] soundNames)
        {
            int loop = 0;
            foreach (var item in sounds)
            {
                soundList.Add(soundNames[loop], sounds[loop]);
                loop++;
            }
            Volume = 0.5f;
            soundActive = true;
        }

        public void playSound(string soundName)
        {
            if (!soundActive)
            {
                return;
            }
            try
            {
                if (soundName == "sprinting")
                {
                    soundList["walking"].Play(Volume, 0.5f, 0);
                }
                else
                {
                    soundList[soundName].Play(Volume, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("No Sound found for key 'soundName'");
            }
        }
    }
}
