using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame
{
    class Arrow
    {
        Vector2 pos;
        public Vector2 Pos { get { return pos; } }
        float angle;
        public float Angle { get { return angle; } }

        public void setUp(Vector2 position, Vector2 toPos)
        {
            pos = position;
            Vector2 newVec = toPos - pos;

            angle = (float)(Math.Atan(newVec.Y / newVec.X));
        }

        public void updateArrow(Vector2 toPos)
        {           
            
            Vector2 direction = toPos - pos;
            float rotation = (float)Math.Atan2(-direction.Y, -direction.X);

            rotation += (float)(Math.PI);

            angle = rotation;

        }

        public void drawArrow(SpriteBatch spriteBatch, Texture2D arrow, Color tint)
        {

            spriteBatch.Draw(texture: arrow, position: new Vector2(pos.X * 2 - 75, pos.Y * 2 - 75), color: tint, rotation: angle, origin: new Vector2(0,0));

        }

    }
}
