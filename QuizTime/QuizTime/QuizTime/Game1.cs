using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;


namespace QuizTime
{
    /// <summary>
    /// Questo è il tipo principale per il gioco
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        #endregion

        #region Initialization

        public Game1()
        {
            AudioManager.Initialize(this);

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // La frequenza fotogrammi è di 30 fps per impostazione predefinita per Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Estendere la durata della batteria in caso di blocco.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            graphics.IsFullScreen = true;

            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 480;

            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            // Create a new instance of the Screen Manager
            screenManager = new ScreenManager(this);
            

            if(PhoneApplicationService.Current.StartupMode == StartupMode.Launch)
            {
                    screenManager.AddScreen(new BackgroundScreen("titleBackground"));
                    screenManager.AddScreen(new MainMenuScreen());
            }

            // Subscribe to the application's lifecycle events
            PhoneApplicationService.Current.Activated += GameActivated;
            PhoneApplicationService.Current.Deactivated += GameDeactivated;
            PhoneApplicationService.Current.Closing += GameClosing;
            PhoneApplicationService.Current.Launching += GameLaunching;

            //screenManager.AddScreen(new GameplayScreen());
            //screenManager.AddScreen(new LoadingScreen());
            Components.Add(screenManager);
        }

        #endregion

        #region Load

        protected override void LoadContent()
        {
            //AudioManager.LoadSounds();
            //AudioManager.LoadMusic();


            base.LoadContent();
        }

        #endregion

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            base.Draw(gameTime);
        }

        #endregion

        #region Tombstoning


        void GameDeactivated(object sender, DeactivatedEventArgs e)
        {

        }


        void GameActivated(object sender, ActivatedEventArgs e)
        {

        }


        void GameClosing(object sender, ClosingEventArgs e)
        {
            HighScoreScreen.SaveHighscore();
        }


        void GameLaunching(object sender, LaunchingEventArgs e)
        {
            HighScoreScreen.LoadHighscores();
        }

        #endregion
    }
}
