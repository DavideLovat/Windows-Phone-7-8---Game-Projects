using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class Engine : InGameComponent
    {
        #region Fields

        protected Texture2D engineOnTexture;
        protected Texture2D engineOffTexture;
        protected Texture2D activeTexture;
        protected ScoreBar scoreBar;
        protected bool on;
        protected bool isIncrease;

        #endregion

        #region Properties

        public override Vector2 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;

                scoreBar.Position = value +
                    new Vector2(activeTexture.Width / 2, activeTexture.Height) +
                    new Vector2(-scoreBar.Width() / 2, 0);
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width(), Height());
            }
        }

        public override Rectangle CentralCollisionArea
        {
            get
            {
                Rectangle bounds = Bounds;
                int height = (int)Bounds.Height / 10 * 5;
                int width = (int)Bounds.Width;

                int offsetY = ((int)Bounds.Height - height) / 2;
                int offsetX = ((int)Bounds.Width - width) / 2;

                return new Rectangle(
                    (int)Bounds.X + offsetX,
                    (int)Bounds.Y + offsetY,
                    width,
                    height);
            }
        }

        public bool On
        {
            get { return on; }
            set
            {
                on = value;
                if (on)
                    activeTexture = engineOnTexture;
                else
                    activeTexture = engineOffTexture;
            }
        }

        public bool IsEnegryFull
        {
            get { return scoreBar.isMaxCurrentValue; }
        }

        #endregion

        public Engine(Texture2D engineOnTexture, Texture2D engineOffTexture, ScoreBar scoreBar, bool on)
            :base()
        {
            this.engineOnTexture = engineOnTexture;
            this.engineOffTexture = engineOffTexture;
            this.scoreBar = scoreBar;
            On = on;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsEnegryFull)
            {
                if (!On)
                    On = true;
            }
            else
            {
                if (On)
                    On = false;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(activeTexture, Position, SourceRectangle, Color, Rotation, Origin, Scale, Effects, LayerDepth);           
            scoreBar.Draw(spriteBatch, gameTime);
        }

        public override int Width()
        {
            return activeTexture.Width;
        }

        public override int Height()
        {
            return activeTexture.Height;
        }

        public void IncreaseEnergy(int amount)
        {
            scoreBar.IncreaseCurrentValue(amount);
        }
    }
}
