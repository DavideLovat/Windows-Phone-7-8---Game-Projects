using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ZoneGame
{
    class Grass : InanimateGameComponent
    {
        public override Rectangle LayerDepthRectangle
        {
            get
            {
                return base.LayerDepthRectangle;
            }
        }

        public override Rectangle BottomCollisionArea
        {
            get
            {
                return Bounds;
            }
        }

        public Grass(Texture2D text, Vector2 worldSize)
            : base(text, worldSize)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(idleComponentTexture, Position, SourceRectangle, Color, Rotation, Origin, Scale, Effects, 0f);
        }
    }
}
