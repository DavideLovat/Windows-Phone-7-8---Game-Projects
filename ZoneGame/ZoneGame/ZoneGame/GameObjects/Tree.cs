using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ZoneGame
{
    class Tree : InanimateGameComponent
    {
        #region Bounds

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
                int height = 1;//(int)Bounds.Height / 10 * 1;
                int width = 10;//(int)Bounds.Width / 10 * 1;

                int offsetY = 171;
                int offsetX = 52;

                return new Rectangle((int)Bounds.X + offsetX, (int)Bounds.Y + offsetY, width, height);
            }
        }

        public override Rectangle BottomCollisionArea
        {
            get
            {
                Rectangle bounds = Bounds;
                int height = 10;//(int)Bounds.Height / 10 * 1;
                int width = 28;//(int)Bounds.Width / 10 * 1;

                int offsetY = 190;
                int offsetX = 46;

                return new Rectangle((int)Bounds.X + offsetX, (int)Bounds.Y + offsetY, width, height); 
            }
        }

        #endregion

        #region Initialization

        public Tree(Texture2D idleComponentTexture, Vector2 worldSize)
            : base(idleComponentTexture, worldSize)
        {
        }


        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            //spriteBatch.Draw(GameplayScreen.blank, CentralCollisionArea, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            //spriteBatch.Draw(GameplayScreen.blank, BottomCollisionArea, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
        #endregion
    }
}
