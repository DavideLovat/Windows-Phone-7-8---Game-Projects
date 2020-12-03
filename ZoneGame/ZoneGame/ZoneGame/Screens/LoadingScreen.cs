using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace ZoneGame
{
    class LoadingScreen : GameScreen
    {
        Text LoadingText;
        Text DotText;

        SpriteFont font;

        float speed = 0.2f;

        bool isLoading;
        bool isReady;

        GameScreen screen;
        Thread loadingThread;

        TimeSpan timeToWait = TimeSpan.FromSeconds(0.5);
        TimeSpan timeElapsed;

        StringBuilder strBuilder = new StringBuilder();

        int levelNumber = 0;

        public LoadingScreen(GameScreen screen)
        {
            this.screen = screen;

            EnabledGestures = GestureType.Tap;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            LanguageDefinitions = Reader.LoadLanguage("Loading");

            font = contentDynamic.Load<SpriteFont>("Fonts/MenuFont");
            LoadingText = new Text("",font);
            DotText = new Text("", font);
            screen.ScreenManager = ScreenManager;
        }

        public override void  UnloadContent()
        {
 	         base.UnloadContent();
        }

        public override void HandleInput(InputState input)
        {
            if (isReady)
            {
                if (input.Gestures.Count > 0)
                {
                    if (input.Gestures[0].GestureType == GestureType.Tap)
                    {
                        PlayScreen();
                    }
                }
            }

            base.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if(isReady)
                return;
            if (!isLoading && loadingThread == null)
            {
                LoadResources();
            }
            else if (isLoading && loadingThread != null)
            {
                if (loadingThread.ThreadState == ThreadState.Stopped && !isExiting)
                {
                    isReady = true;
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            timeElapsed += gameTime.ElapsedGameTime;

            string dots = "...";

            if (isReady)
            {
                LoadingText.TextContents = LanguageDefinitions["Continue"];
            }
            else
            {
                LoadingText.TextContents = LanguageDefinitions["Loading"];
            }

            LoadingText.Position = new Vector2(
                viewport.Width - LoadingText.Width() - DotText.Font.MeasureString(dots).X,
                viewport.Height - LoadingText.Font.LineSpacing);
            LoadingText.Color = Color.Yellow;
            DotText.Position = LoadingText.Position + new Vector2(LoadingText.Width(), 0); 
            if (!isReady)
            {
                if (strBuilder.Length > 3)
                {
                    strBuilder = new StringBuilder();                    
                }
                else if (timeElapsed > timeToWait)
                {
                    strBuilder.Append(".");
                    timeElapsed -= timeToWait; 
                }

                DotText.TextContents = strBuilder.ToString();
            }

            spriteBatch.Begin();

            LoadingText.Draw(spriteBatch, gameTime);
            if(!isReady)
                DotText.Draw(spriteBatch, gameTime);

            spriteBatch.End();
        }

        #region Private Methods

        private void LoadResources()
        {
            loadingThread = new Thread(new ThreadStart(screen.LoadAssets));
            isLoading = true;
            loadingThread.Start();
        }

        protected virtual void PlayScreen()
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            if (this.screen is GameplayScreen)
            {
                GameplayScreen gmpScreen = (GameplayScreen)this.screen;
                gmpScreen.IsActive = true;
            }

            ScreenManager.AddScreen(this.screen);
        }

        protected virtual void OnCancel() { }

        #endregion
    }
}
