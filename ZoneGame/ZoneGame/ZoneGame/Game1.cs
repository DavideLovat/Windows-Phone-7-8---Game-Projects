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


namespace ZoneGame
{
    /// <summary>
    /// Questo è il tipo principale per il gioco
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        public Game1()
        {
            // Inizializa suono
            AudioManager.Initialize(this);

            graphics = new GraphicsDeviceManager(this);            
            Content.RootDirectory = "Content";

            // La frequenza fotogrammi è di 30 fps per impostazione predefinita per Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Estendere la durata della batteria in caso di blocco.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            // Imposta la modalità schermo intero
            graphics.IsFullScreen = true;
            /*
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 800;

            graphics.SupportedOrientations = DisplayOrientation.Default;
            */
            // Carica le impostazioni di gioco in SettingsManager
            SettingsData SettingsData = MemoryStateManager.LoadSettingsData();            

            // Crea una nuova istanza di ScreenManager
            screenManager = new ScreenManager(this);

            // Aggiunge gli screen di Background e MainMenu allo screen manager 
            //screenManager.AddScreen(new BackgroundScreen("titleScreen"));

            //screenManager.AddScreen(new MainMenuScreen());

            //screenManager.AddScreen(new OptionsScreen());

            //screenManager.AddScreen(new IntroductionScreen());

            //screenManager.AddScreen(new CreditsScreen());

            //screenManager.AddScreen(new HighScoreScreen());

            //screenManager.AddScreen(new GameplayScreen());

            Components.Add(screenManager);

            // Subscribe to the application's lifecycle events
            PhoneApplicationService.Current.Activated += GameActivated;
            PhoneApplicationService.Current.Deactivated += GameDeactivated;
            PhoneApplicationService.Current.Closing += GameClosing;
            PhoneApplicationService.Current.Launching += GameLaunching;
        }

        /// <summary>
        /// Consente al gioco di eseguire tutte le operazioni di inizializzazione necessarie prima di iniziare l'esecuzione.
        /// È possibile richiedere qualunque servizio necessario e caricare eventuali
        /// contenuti non grafici correlati. Quando si chiama base.Initialize, tutti i componenti vengono enumerati
        /// e inizializzati.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent verrà chiamato una volta per gioco e costituisce il punto in cui caricare
        /// tutto il contenuto.
        /// </summary>
        protected override void LoadContent()
        {
            AudioManager.LoadSounds();
            AudioManager.LoadMusic();
            // Aggiunge gli screen di Background e MainMenu allo screen manager
            screenManager.AddScreen(new BackgroundScreen("titleScreen"));
            screenManager.AddScreen(new MainMenuScreen());
        }

        /// <summary>
        /// UnloadContent verrà chiamato una volta per gioco e costituisce il punto in cui scaricare
        /// tutto il contenuto.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Consente al gioco di eseguire la logica per, ad esempio, aggiornare il mondo,
        /// controllare l'esistenza di conflitti, raccogliere l'input e riprodurre l'audio.
        /// </summary>
        /// <param name="gameTime">Fornisce uno snapshot dei valori di temporizzazione.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Viene chiamato quando il gioco deve disegnarsi.
        /// </summary>
        /// <param name="gameTime">Fornisce uno snapshot dei valori di temporizzazione.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: aggiungere qui il codice di disegno

            base.Draw(gameTime);
        }

        #region Tombstoning

        /// <summary>
        /// Saves the full state to the state object and the persistent state to 
        /// isolated storage.
        /// </summary>
        void GameDeactivated(object sender, DeactivatedEventArgs e)
        {
            SaveToStateObject();

            if (PhoneApplicationService.Current.State.ContainsKey("CurrentLevel"))
            {
                GameData gameData = new GameData();
                gameData.currentLevel = (int)PhoneApplicationService.Current.State["CurrentLevel"];
                MemoryStateManager.SaveGameData(gameData);
                PhoneApplicationService.Current.State.Remove("CurrentLevel");
            }

            SettingsData settingsData = new SettingsData();
            settingsData.language = SettingsManager.Language;
            settingsData.soundEnabled = AudioManager.IsActive;

            MemoryStateManager.SaveSettingsData(settingsData);

            HighScoreScreen.SaveHighscore();
        }

        /// <summary>
        /// Loads the full state from the state object.
        /// </summary>
        void GameActivated(object sender, ActivatedEventArgs e)
        {
            GameData gameData = MemoryStateManager.LoadGameData();
            SettingsData settingsData = MemoryStateManager.LoadSettingsData();

            AudioManager.IsActive = settingsData.soundEnabled;
            SettingsManager.Language = settingsData.language;

            HighScoreScreen.LoadHighscores();
        }

        /// <summary>
        /// Saves persistent state to isolated storage.
        /// </summary>
        void GameClosing(object sender, ClosingEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("CurrentLevel"))
            {
                GameData gameData = new GameData();
                gameData.currentLevel = (int)PhoneApplicationService.Current.State["CurrentLevel"];
                MemoryStateManager.SaveGameData(gameData);
                PhoneApplicationService.Current.State.Remove("CurrentLevel");
            }
            else
            {
                CleanIsolatedStorage();
            }

            SettingsData settingsData = new SettingsData();
            settingsData.language = SettingsManager.Language;
            settingsData.soundEnabled = AudioManager.IsActive;

            MemoryStateManager.SaveSettingsData(settingsData);

            HighScoreScreen.SaveHighscore();
        }


        /// <summary>
        /// Loads persistent state from isolated storage.
        /// </summary>
        void GameLaunching(object sender, LaunchingEventArgs e)
        {
            GameData gameData = MemoryStateManager.LoadGameData();
            SettingsData settingsData = MemoryStateManager.LoadSettingsData();

            AudioManager.IsActive = settingsData.soundEnabled;
            SettingsManager.Language = settingsData.language;

            HighScoreScreen.LoadHighscores();
        }

        #endregion

        #region Helpers functionality
        /// <summary>
        /// Saves current gameplay progress to the state object.
        /// </summary>
        private void SaveToStateObject()
        {
            // Get the gameplay screen object
            GameplayScreen gameplayScreen = GetGameplayScreen();

            if (null != gameplayScreen)
            {
                // If gameplay screen object found save current game progress to 
                // the state object
                PhoneApplicationService.Current.State["CurrentLevel"] =
                    gameplayScreen.CurrentLevel;
            }
        }

        /// <summary>
        /// Clean isolated storage from previously saved information.
        /// </summary>
        private void CleanIsolatedStorage()
        {
            using (IsolatedStorageFile isolatedStorageFile
                = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isolatedStorageFile.DeleteFile("GameData.sav");
            }

        }

        /// <summary>
        /// Finds a gameplay screen objects among all screens and returns it.
        /// </summary>
        /// <returns>A gameplay screen instance, or null if none 
        /// are available.</returns>
        private GameplayScreen GetGameplayScreen()
        {
            var screens = screenManager.GetScreens();

            foreach (var screen in screens)
            {
                if (screen is GameplayScreen)
                {
                    return screen as GameplayScreen;
                }
            }

            return null;
        }

        #endregion
    }
}
