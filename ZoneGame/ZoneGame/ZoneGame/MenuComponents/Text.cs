using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public class Text:Component
    {
        #region Fields

        string textContents;
        SpriteFont font;

        #endregion

        #region Properties

        public string TextContents
        {
            get { return textContents; }
            set { textContents = value; }
        }

        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        #endregion

        #region Initialization

        public Text(string textContents, SpriteFont font)
            :base()
        {
            this.textContents = textContents;
            this.font = font;
        }

        public Text(int index, string textContents, SpriteFont font)
            :base(index)
        {
            this.textContents = textContents;
            this.font = font;
        }

        #endregion

        #region Methods

        public override void Update(GameTime gameTime) { }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {            
            if ((font != null) && !String.IsNullOrEmpty(textContents))
            {
                spriteBatch.DrawString(font, textContents, position, color * alphaChannel, rotation, origin, scale, effects, layerDepth); ;
            }
        }

        public override int Height()
        {
            //return (int)((float)font.LineSpacing * scale);
            return (int)(font.MeasureString(textContents).Y * scale);
        }

        public override int Width()
        {
            return (int)(font.MeasureString(textContents).X * scale);
        }

        #endregion
    }
}
