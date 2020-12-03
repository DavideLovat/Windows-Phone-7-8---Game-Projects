using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public class Bullet : InanimateGameComponent
    {
        public override Rectangle LayerDepthRectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, 1, 33);
            }
        }

        private Vector2 velocity;

        public Bullet(Texture2D idleComponentTexture, Vector2 worldSize, Vector2 pos, Vector2 vel, Color col)
            :base(idleComponentTexture, worldSize)
        {
            Position = pos;
            velocity = vel;
            color = col;
        }

        public override void Update(GameTime gamTime)
        {
            Position += velocity;
        }

        public void PlayBulletCollision()
        {
            AudioManager.PlaySound("bulletAmbientCollision", false, 0.4f);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(idleComponentTexture, Position, SourceRectangle, Color, Rotation, Origin, Scale, Effects, CalcolateLayerDepth());
        }
    }
}
