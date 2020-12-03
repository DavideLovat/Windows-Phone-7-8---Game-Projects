using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.GamerServices;

namespace QuizTime
{
    class PauseScreen : BasicMenu
    {
        #region Fields

        ImageSelectable titleContinueButton, titleQuitButton;

        bool isResuming;
        bool checkHighscore = false;
        bool moveToHighScore = false;
        bool moveToMainMenu = false;

        #endregion

        #region Initialization

        public PauseScreen(bool isResuming)
        {
            this.isResuming = isResuming;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            AudioManager.PlaySound("gate");

            titleContinueButton = new ImageSelectable(contentDynamic.Load<Texture2D>("Textures/Buttons/titleButton_Continue"));
            titleQuitButton = new ImageSelectable(contentDynamic.Load<Texture2D>("Textures/Buttons/titleButton_Quit"));

            titleContinueButton.Selected += ContinueButtonSelected;
            titleQuitButton.Selected += QuitButtonSelected;

            AddSideBarComponent(titleContinueButton);
            AddSideBarComponent(titleQuitButton);
            /*
            if (!isResuming)
            {
                AudioManager.PauseResumeSounds(false);
            }

            if (isResuming && !AudioManager.IsInitialized)
            {
                AudioManager.LoadSounds();
            }*/
            //AudioManager.PlaySound("menu");
        }

        #endregion


        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (checkHighscore && (!Guide.IsVisible))
            {
                checkHighscore = false;

                var gameplayScreen = GetGameplayScreen();

                var rightAnswers = gameplayScreen.RightAnswers;

                if (HighScoreScreen.IsInHighscores(rightAnswers))
                {
                    // Show the device's keyboard to record a high score
                    Guide.BeginShowKeyboardInput(PlayerIndex.One,
                        "Player Name", "Insert your name (max 15 characters)!", "Player",
                        ShowHighscorePromptEnded, rightAnswers);
                }
                else
                {
                    moveToMainMenu = true;
                }
            }
            else if (moveToHighScore)
            {
                ReplaceAllScreens(
                    new List<GameScreen>() 
                    { 
                        new BackgroundScreen("highscoreBackground"),
                        new HighScoreScreen()
                    });
            }
            else if (moveToMainMenu)
            {
                ReplaceAllScreens(
                    new List<GameScreen>()
                    {
                        new BackgroundScreen("titleBackground"),
                        new MainMenuScreen(),
                    });
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }


        #endregion

        #region Methods

        void ContinueButtonSelected(object sender, EventArgs e)
        {
            if (!isResuming)
            {
                // Resume sounds and activate the gameplay screen
                AudioManager.PauseResumeSounds(true);

                var screens = ScreenManager.GetScreens();

                foreach (GameScreen screen in screens)
                {
                    if (!(screen is GameplayScreen))
                    {
                        screen.ExitScreen();
                    }
                }

                (ScreenManager.GetScreens()[0] as GameplayScreen).IsActive = true;
            }
            else
            {
                // Since we are resuming the game, go to the loading screen which will
                // in turn initialize the gameplay screen
                foreach (GameScreen screen in ScreenManager.GetScreens())
                    screen.ExitScreen();

                ScreenManager.AddScreen(new LoadingScreen());
            }
        }

        void QuitButtonSelected(object sender, EventArgs e)
        {
            OnCancel();
        }

        private void ShowHighscorePromptEnded(IAsyncResult result)
        {
            string playerName = Guide.EndShowKeyboardInput(result);

            int rightAnswers = (int)result.AsyncState;

            if (playerName != null)
            {
                if (playerName.Length > 15)
                    playerName = playerName.Substring(0, 15);

                HighScoreScreen.PutHighScore(playerName, rightAnswers);
            }

            moveToHighScore = true;            
        }
       
        protected override void OnCancel()
        {
            AudioManager.StopSounds();
            checkHighscore = true;
        }

        private GameplayScreen GetGameplayScreen()
        {
            var screens = ScreenManager.GetScreens();

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
