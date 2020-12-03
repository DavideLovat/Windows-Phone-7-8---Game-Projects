using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class ClockBar : AnimatedBar
    {
        #region Fields

        Texture2D clockBarTexture;

        public Texture2D ClockBarTexture
        {
            get { return clockBarTexture; }
            set { clockBarTexture = value; }
        }

        TimeSpan maxClockTime;
        TimeSpan clockTime;

        SpriteFont font;

        bool isTimeOut;
        bool isOutOfTime;

        #endregion

        #region Properties

        public TimeSpan MaxClockTime
        {
            get { return maxClockTime; }
            set 
            { 
                maxClockTime = value;
                ResetClock();
            }
        }

        public TimeSpan ClockTime
        {
            get { return clockTime; }
            private set { clockTime = value; }
        }

        public bool IsTimeOut
        {
            get { return isTimeOut; }
            set { isTimeOut = value; }
        }

        public bool IsOutOfTime
        {
            get { return isOutOfTime; }
        }

        #endregion

        #region Initialization

        public ClockBar(SpriteFont font, Texture2D clockBarTexture, TimeSpan maxClockTime, bool isTimeOut, Vector2 closedPosition, Vector2 openedPosition, bool barIsOpened)
            : base(closedPosition, openedPosition, barIsOpened)
        {
            MaxClockTime = maxClockTime;
            this.font = font;
            this.clockBarTexture = clockBarTexture;
            this.isTimeOut = isTimeOut;
        }

        public ClockBar(int index, SpriteFont font, Texture2D clockBarTexture, TimeSpan maxClockTime, bool isTimeOut, Vector2 closedPosition, Vector2 openedPosition, bool barIsOpened)
            : base(index, closedPosition, openedPosition, barIsOpened)
        {
            MaxClockTime = maxClockTime;
            this.font = font;
            this.clockBarTexture = clockBarTexture;
            this.isTimeOut = isTimeOut;
        }

        #endregion

        #region Update

        public override void Update(GameScreen screen, GameTime gameTime)
        {
            base.Update(screen, gameTime);

            if (isTimeOut || isOutOfTime)
            {
                return;
            }

            clockTime = TimeSpan.FromSeconds(MathHelper.Clamp(
                                (float)clockTime.TotalSeconds - (float)gameTime.ElapsedGameTime.TotalSeconds, (float)TimeSpan.FromSeconds(0).TotalSeconds, (float)maxClockTime.TotalSeconds));
            
            if (clockTime <= TimeSpan.Zero)
            {
                isOutOfTime = true;
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            base.Draw(screen, gameTime);

            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            spriteBatch.Draw(clockBarTexture, Position, null, Color.White * alphaChannel, 0, Vector2.Zero, scale, effects, 0);
            spriteBatch.DrawString(font, UpdateClockText(), UpdateClockTextPosition(), Color);
        }

        #endregion

        #region Public Methods

        public override int Height(GameScreen screen)
        {
            return (int)((float)clockBarTexture.Height * scale);
        }

        public override int Width(GameScreen screen)
        {
            return (int)((float)clockBarTexture.Width * scale);
        }

        public void ResetClock()
        {
            clockTime = maxClockTime;
            isOutOfTime = false;
        }

        #endregion

        #region Private Methods

        private String UpdateClockText()
        {
            return String.Format("{0:00}:{1:00}", clockTime.Minutes, clockTime.Seconds);
        }

        private Vector2 UpdateClockTextPosition()
        {
            // Clock text position
            Vector2 textSize = font.MeasureString(UpdateClockText());

            return new Vector2(
                Position.X + (clockBarTexture.Width - textSize.X) / 2,
                Position.Y + clockBarTexture.Height - font.LineSpacing);
        }

        #endregion

    }
}
