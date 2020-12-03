using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class Paragraph : Component
    {
        public new Vector2 Position
        {
            get 
            {
                return position;
            }

            set 
            {
                position = value;
                paragraphBounds.X = (int)position.X;
                paragraphBounds.Y = (int)position.Y;
            }
        }

        public virtual Rectangle ParagraphBounds
        {
            get { return paragraphBounds; }
            set 
            { 
                paragraphBounds = value;
                position = new Vector2(paragraphBounds.X, paragraphBounds.Y);
            }
        }
        protected Rectangle paragraphBounds;

        public virtual SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }
        protected SpriteFont font;

        public virtual String Text
        {
            get { return text; }
            set { text = value; }
        }
        protected String text;

        public Paragraph(SpriteFont font)
            : this(0, font, Rectangle.Empty, String.Empty)
        {            
        }

        public Paragraph(SpriteFont font, Rectangle bounds, String text)
            : this(0, font, bounds, text)
        {
        }

        public Paragraph(int index, SpriteFont font)
            : this(index, font, Rectangle.Empty, String.Empty)
        {            
        }        

        public Paragraph(int index, SpriteFont font, Rectangle bounds, String text)
            :base(index)
        {
            this.font = font;
            this.paragraphBounds = bounds;
            this.text = text;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(font, parseText(text), new Vector2(paragraphBounds.X, paragraphBounds.Y), Color);
        }

        public override int Width()
        {
            return paragraphBounds.Width;
        }

        public override int Height()
        {
            return paragraphBounds.Height;
        }

        #region Protected Methods

        protected virtual String parseText(String Text)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = text.Split(' ');

            foreach (String world in wordArray)
            {
                if (font.MeasureString(line + world).Length() > paragraphBounds.Width)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }

                line = line + world + ' ';
            }

            return returnString + line;
        }

        #endregion
    }
}
