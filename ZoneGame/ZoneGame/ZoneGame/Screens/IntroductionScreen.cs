using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace ZoneGame
{
    class IntroductionScreen : GameScreen
    {
        Texture2D blankTexture;
        Texture2D badmoodTexture;

        Rectangle blankDimension;

        Vector2 badmoodPosition;

        SpriteFont font14px;    //Debug

        float alphaChannel = 0f;
        float transAlpha;
        float badmoodScale = 1.5f;

        bool isLogoVisible;
        bool isScreenCreated;

        Color badmoodColor = Color.White;

        TimeSpan timeToWait = TimeSpan.FromSeconds(3);
        TimeSpan timeElapsed;

        public override void LoadContent()
        {
            if (contentDynamic == null)
                contentDynamic = new ContentManager(ScreenManager.Game.Services, "Content");

            blankTexture = contentDynamic.Load<Texture2D>("Textures/blank");
            badmoodTexture = contentDynamic.Load<Texture2D>("Textures/Logos/logoBadMood_White");

            Viewport viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            badmoodPosition = new Vector2((viewport.Width - badmoodTexture.Width * badmoodScale) / 2, (viewport.Height - badmoodTexture.Height * badmoodScale) / 2);
            blankDimension = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);

            // Debug elements
            font14px = contentDynamic.Load<SpriteFont>("Fonts/font14px");
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (isLogoVisible)
            {
                if (timeToWait.TotalSeconds > 0)
                {
                    timeToWait -= gameTime.ElapsedGameTime;
                }
                if (timeToWait.Seconds == 0)
                {
                    if (!isScreenCreated)
                    {
                        ReplaceAllScreens(new List<GameScreen>() { new BackgroundScreen("titleScreen"), new MainMenuScreen() });
                        isScreenCreated = true;
                    }
                    
                }
                return;
            }

            if (alphaChannel == 1)
            {
                isLogoVisible = true;
                return;
            }

            timeElapsed += gameTime.ElapsedGameTime;

            transAlpha = (1f / 3f) * (float)gameTime.ElapsedGameTime.TotalSeconds ;

            alphaChannel += transAlpha;

            alphaChannel = MathHelper.Clamp(alphaChannel, 0f, 1f);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw Debug elements
            Vector2 debugPosition = Vector2.Zero;
            List <String> debugData = new List<string>();
            debugData.Add(String.Format("Alpha Time: Seconds{0}, Milliseconds{1}", timeElapsed.Seconds, timeElapsed.Milliseconds));
            debugData.Add(String.Format("Alpha Channel: {0}", alphaChannel));
            debugData.Add(String.Format("Transparent Alpha: {0}", transAlpha ));
            debugData.Add(String.Format("Is Logo Visible: {0}", isLogoVisible));
            debugData.Add(String.Format("Time To Wait: Seconds {0}, Milliseconds {1},", timeToWait.Seconds, timeToWait.Milliseconds));

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            for(int i=0; i<debugData.Count; i++)
            {
                ScreenManager.SpriteBatch.DrawString(font14px, debugData[i], debugPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                debugPosition += new Vector2(0,20);
            }

            // Draw Background Color
            ScreenManager.SpriteBatch.Draw(blankTexture, blankDimension, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Draw logo
            ScreenManager.SpriteBatch.Draw(badmoodTexture, badmoodPosition, null, badmoodColor * alphaChannel, 0f, Vector2.Zero, badmoodScale, SpriteEffects.None, 0.1f);

            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
