using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame
{
    class MuzzleFlash
    {
        int ticksLeft;
        public int TicksLeft { get { return ticksLeft; } }
        private float angle;
        public Vector2 Pos { get; set; }
        private int flashType;
        Random ran = new Random();
        public MuzzleFlash(Vector2 pos, float rotation)
        {
            angle = (float)(rotation + (Math.PI/2));
            ticksLeft = 2;
            Pos = pos;
            flashType = ran.Next(5);
        }

        public void DrawFlash(SpriteBatch spriteBatch, Texture2D[] textures)
        {
            spriteBatch.Draw(texture: textures[flashType + 1], position: Pos, color: Color.White, rotation: angle, origin: new Vector2(-45, 30));
            ticksLeft--;
        }
    }
}
