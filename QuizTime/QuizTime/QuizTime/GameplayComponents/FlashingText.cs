using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class FlashingText : Text
    {
        TimeSpan flashTime;
        TimeSpan flashingDuration = TimeSpan.FromSeconds(1);

        public TimeSpan FlashingDuration
        {
            get { return flashingDuration; }
            set { flashingDuration = value; }
        }

        public bool IsFlashing
        {
            get;
            set;
        }

        public FlashingText(string text, SpriteFont font)
            : base(text, font)
        {

        }

        public FlashingText(int index, string text, SpriteFont font)
            : base(index, text, font)
        {

        }

        public override void Update(GameScreen screen, GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            if (gameTime.TotalGameTime != flashTime)
            {
                if (flashTime + flashingDuration < gameTime.TotalGameTime)
                {
                    flashTime = gameTime.TotalGameTime;
                    IsFlashing = !IsFlashing;
                }
            }
        }

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            // Draw textContents, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            //origin = new Vector2(0, font.LineSpacing / 2);

            if (IsFlashing)
            {
                if ((Font != null) && !String.IsNullOrEmpty(TextContents))
                {
                    spriteBatch.DrawString(Font, TextContents, position, color * alphaChannel, rotation, origin, scale, effects, 0); ;
                }
            }
        }
    }
}
