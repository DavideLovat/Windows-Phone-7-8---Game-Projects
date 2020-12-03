using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    public class Image:Component
    {
        #region Fields

        Texture2D imageContents;

        #endregion

        #region Properties

        public Texture2D ImageContents
        {
            get { return imageContents; }
            set { imageContents = value; }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y,
                                    imageContents.Width, imageContents.Height);
            }
        }

        #endregion

        #region Initialization

        public Image(Texture2D imageContents)
            :base()
        {
            this.imageContents = imageContents;
        }

        public Image(int index, Texture2D imageContents)
            : base(index)
        {
            this.imageContents = imageContents;
        }

        #endregion

        #region Methods

        public override void Update(GameScreen screen, GameTime gameTime) 
        {
            base.Update(screen, gameTime);
        }

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            base.Draw(screen, gameTime);
            // Draw texture.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            if(imageContents != null)
                spriteBatch.Draw(imageContents, position, sourceRectangle, color * alphaChannel, rotation, origin, scale, effects, 0);
        }

        public override int Height(GameScreen screen)
        {
            return (int)((float)imageContents.Height * scale);
        }

        public override int Width(GameScreen screen)
        {
            return (int)((float)imageContents.Width * scale);
        }

        #endregion


    }
}
