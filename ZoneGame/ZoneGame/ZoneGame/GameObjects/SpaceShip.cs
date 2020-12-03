using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class SpaceShip : InanimateGameComponent
    {
        public override Rectangle LayerDepthRectangle
        {
            get
            {
                return Bounds;
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                return base.Bounds;
            }
        }

        public override Rectangle CentralCollisionArea
        {
            get
            {
                Rectangle bounds = Bounds;
                int height = (int)Bounds.Height / 10 * 3;
                int width = (int)Bounds.Width / 10 * 9;

                int offsetY = ((int)Bounds.Height - height) / 2;
                int offsetX = ((int)Bounds.Width - width) / 2;

                return new Rectangle((int)Bounds.X + offsetX, (int)Bounds.Y + offsetY, width, height);
            }
        }

        public SpaceShip(Texture2D idleComponentTexture, Vector2 worldSize)
            : base(idleComponentTexture, worldSize)
        { }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);            
        }
    }
}
