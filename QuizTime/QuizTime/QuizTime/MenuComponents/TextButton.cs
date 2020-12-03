using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    public class TextButton : TextSelectable
    {
        #region Fields 
        
        Texture2D buttonTexture;

        float alphaTexture = 1f;
        float alphaText = 1f;

        #endregion

        #region Properties

        public Texture2D ButtonTexture
        {
            get { return buttonTexture; }
            set { buttonTexture = value; }
        }

        public float AlphaTexture
        {
            get { return alphaTexture; }
            set { alphaTexture = value; }
        }

        public float AlphaText
        {
            get { return alphaText; }
            set { alphaText = value; }
        }

        public new float AlphaChannel
        {
            get { return alphaChannel; }
            set 
            {
                alphaChannel = value;
                alphaTexture = alphaChannel;
                alphaText = alphaChannel;
            }
        }

        #endregion

        #region Initialization

        public TextButton(string textContents, SpriteFont font, Texture2D buttonTexture)
            : base(textContents, font)
        {
            this.buttonTexture = buttonTexture;
        }

        public TextButton(int index, string textContents, SpriteFont font, Texture2D buttonTexture)
            :base(index,textContents,font)
        {
            this.ButtonTexture = buttonTexture;
        }

        #endregion

        #region Draw

        public override void Update(GameScreen screen, GameTime gameTime)
        {
            base.Update(screen, gameTime);
        }

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            base.Draw(screen, gameTime);

            Color tintColor = Color.White;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            
            if(buttonTexture != null)
            spriteBatch.Draw(buttonTexture, position, null, tintColor * alphaTexture, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            if (Font != null && !String.IsNullOrEmpty(TextContents))
            spriteBatch.DrawString(Font, TextContents, getTextPosition(screen),
                color * alphaText, rotation, Vector2.Zero, scale, SpriteEffects.None, 0); 
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public override int Height(GameScreen screen)
        {
            return (int)((float)buttonTexture.Height * scale);
            //return (int)screen.ScreenManager.Font.MeasureString(Text).Y;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public override int Width(GameScreen screen)
        {
            return (int)((float)buttonTexture.Width * scale);
            //return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }

        #endregion

        #region Private Methods

        private Vector2 getTextPosition(GameScreen screen)
        {
            Vector2 textPosition = Vector2.Zero;
            Vector2 textSize = Font.MeasureString(TextContents);
            if (Scale == 1f)
            {

                textPosition = position + new Vector2(
                    (float)Math.Floor((buttonTexture.Width - textSize.X) / 2),
                    (float)Math.Floor((buttonTexture.Height - textSize.Y) / 2));
            }
            else
            {
                textPosition = position + new Vector2(
                    (float)Math.Floor((buttonTexture.Width - textSize.X) / 2) * scale,
                    (float)Math.Floor((buttonTexture.Height - (textSize.Y - textSize.Y * scale) / 2)));
            }

            return textPosition;
        }

        #endregion
    }
}
