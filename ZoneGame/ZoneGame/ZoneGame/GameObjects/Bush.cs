using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class Bush : InanimateGameComponent
    {
        public override Rectangle LayerDepthRectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, idleComponentTexture.Width, idleComponentTexture.Height);
            }
        }

        public override Rectangle BottomCollisionArea
        {
            get
            {
                Rectangle bounds = Bounds;
                int height = 1;//(int)Bounds.Height / 10 * 1;
                int width = 72;//(int)Bounds.Width / 10 * 1;

                int offsetY = bounds.Height - height;
                int offsetX = 2;

                return new Rectangle((int)Bounds.X + offsetX, (int)Bounds.Y + offsetY, width, height); 
            }
        }

        public Bush(Texture2D text, Vector2 worldSize)
            : base(text, worldSize)
        {
        }
    }
}
