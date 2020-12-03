using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
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

        public override void Update(GameTime gameTime) { }

        public override void Draw( SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw texture.            
            if(imageContents != null)
                spriteBatch.Draw(imageContents, position, null, color * alphaChannel, rotation, origin, scale, effects, 0);
        }

        public override int Height()
        {
            return (int)((float)imageContents.Height * scale);
        }

        public override int Width()
        {
            return (int)((float)imageContents.Width * scale);
        }

        #endregion


    }
}
