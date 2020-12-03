using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    public class Text:Component
    {
        #region Fields

        string textContents;
        SpriteFont font;

        #endregion

        #region Properties

        public virtual string TextContents
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

        public override void Update(GameScreen screen, GameTime gameTime) 
        {
            base.Update(screen, gameTime);
        }

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            base.Draw(screen, gameTime);
            // Draw textContents, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            //origin = new Vector2(0, font.LineSpacing / 2);

            if ((font != null) && !String.IsNullOrEmpty(textContents))
            {
                spriteBatch.DrawString(font, textContents, position, color * alphaChannel, rotation, origin, scale, effects, 0); ;
            }
        }

        public override int Height(GameScreen screen)
        {
            //return (int)((float)font.LineSpacing * scale);
            return (int)(font.MeasureString(textContents).Y * scale);
        }

        public override int Width(GameScreen screen)
        {
            return (int)(font.MeasureString(textContents).X * scale);
        }

        #endregion
    }
}
