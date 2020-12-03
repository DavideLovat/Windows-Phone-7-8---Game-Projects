using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class AnswerTextButton : TextSelectable
    {
        #region Fields

        Texture2D neutralTexture, successTexture, failureTexture;

        Texture2D currentTexture;

        int horizontalSpace = 5;

        #endregion

        #region Initialization

        public AnswerTextButton(string textContents, SpriteFont font, Texture2D neutralTexture, Texture2D successTexture, Texture2D failureTexture)
            : base(textContents, font)
        {
            this.neutralTexture = neutralTexture;
            this.successTexture = successTexture;
            this.failureTexture = failureTexture;
            currentTexture = neutralTexture;
        }

        public AnswerTextButton(int index, string textContents, SpriteFont font, Texture2D neutralTexture, Texture2D successTexture, Texture2D failureTexture)
            : base(index, textContents, font)
        {
            this.neutralTexture = neutralTexture;
            this.successTexture = successTexture;
            this.failureTexture = failureTexture;
            currentTexture = neutralTexture;
        }

        #endregion

        #region Draw

        public override void Update(GameScreen screen, GameTime gameTime)
        { }

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            spriteBatch.Draw(currentTexture, position, null, color, rotation, origin, scale, effects, 0f);

            spriteBatch.DrawString(Font, TextContents, getTextPosition(screen),
                    color, 0f, origin, scale, SpriteEffects.None, 0f);                
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public override int Height(GameScreen screen)
        {
            return (int)MathHelper.Max((float)currentTexture.Height * scale , Font.MeasureString(TextContents).Y * scale);
            //return (int)screen.ScreenManager.Font.MeasureString(Text).Y;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public override int Width(GameScreen screen)
        {
            return (int)((float)currentTexture.Width * scale + Font.MeasureString(TextContents).X * scale) + horizontalSpace;
            //return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }

        public void SetNeutral()
        {
            currentTexture = neutralTexture;
        }

        public void SetSuccess()
        {
            currentTexture = successTexture;
        }

        public void SetFailure()
        {
            currentTexture = failureTexture;
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
                    (float)Math.Floor(currentTexture.Width) + horizontalSpace,
                    (float)Math.Floor((currentTexture.Height - textSize.Y) / 2));
            }
            else
            {
                textPosition = position + new Vector2(
                    (float)Math.Floor(currentTexture.Width) * scale + horizontalSpace,
                    (float)Math.Floor(((currentTexture.Height * scale) / 2 - (textSize.Y * scale) / 2)));
            }

            return textPosition;
        }

        #endregion
    }
}
