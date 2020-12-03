using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Phone.Shell;

namespace QuizTime
{
    class LoadingScreen : GameScreen
    {
        Texture2D circleTexture;

        Image circle;

        float speed = 0.2f;

        bool isLoading;

        GameplayScreen gameplayScreen;
        Thread loadingThread;

        public override void LoadContent()
        {
            base.LoadContent();

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            circleTexture = contentDynamic.Load<Texture2D>(@"Textures\Gamescreens\circle");

            circle = new Image(circleTexture);
            circle.Position = new Vector2(viewport.Width / 2, viewport.Height / 2);
            circle.Origin = new Vector2(circle.Width(this) / 2, circle.Height(this) / 2);

            gameplayScreen = new GameplayScreen();
            gameplayScreen.ScreenManager = ScreenManager;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (isLoading && loadingThread != null)
            {
                if (loadingThread.ThreadState == ThreadState.Stopped && !isExiting)
                {
                    // Move on to the gameplay screen
                    foreach (GameScreen screen in ScreenManager.GetScreens())
                        screen.ExitScreen();

                    gameplayScreen.IsActive = true;
                    ScreenManager.AddScreen(gameplayScreen);
                }
            }

            if (!isLoading && loadingThread == null)
            {
                // Start loading the gameplay resources in an additional thread
                loadingThread = new Thread(
                    new ThreadStart(gameplayScreen.LoadAssets));

                isLoading = true;
                loadingThread.Start();
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            circle.Rotation += MathHelper.TwoPi * speed *
            (float)gameTime.ElapsedGameTime.TotalSeconds;
            circle.Rotation %= MathHelper.TwoPi;

            circle.Draw(this, gameTime);

            spriteBatch.End();
        }
    }
}
