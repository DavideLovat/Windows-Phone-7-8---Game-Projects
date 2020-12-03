using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.GamerServices;

namespace ZoneGame
{
    class PauseScreen : BasicMenu
    {
        #region Fields

        string[] names = new string[] { "Continue", "Restart", "Exit"};

        List<TextButton> textButtons = new List<TextButton>();

        #endregion

        #region Initialization

        #endregion

        #region Loading

        public override void LoadContent()
        {
            base.LoadContent();

            LanguageDefinitions = Reader.LoadLanguage("PauseMenu");

            title.TextContents = LanguageDefinitions["Title"];

            InitializeButtons();

            PlayMusic(true, "mainMenuMusic");
        }

        #endregion

        #region Update



        #endregion

        #region Private Methods

        private void InitializeButtons()
        {
            for (int i = 0; i < names.Length; i++)
            {
                if (LanguageDefinitions.Count < i)
                {
                    break;
                }

                textButtons.Add(new TextButton(LanguageDefinitions[names[i]], menuFont, buttonMenuTexture));

                switch (names[i])
                {
                    case "Continue":
                        textButtons[i].Selected += ContinueEntrySelected;
                        break;
                    case "Restart":
                        textButtons[i].Selected += RestartEntrySelected;
                        break;
                    case "Exit":
                        textButtons[i].Selected += ExitEntrySelected;
                        break;
                }

                if (names[i] == "Continue")
                {
                    var screen = GetGameplayScreen();
                    if (!screen.IsUserWon && screen.GameEnded)
                        continue;
                }

                AddVerticalTextButton(textButtons[i]);
            }
        }

        void ContinueEntrySelected(object sender, EventArgs e)
        {
            ContinueGame();
            TestButtons("Continue");
        }

        void RestartEntrySelected(object sender, EventArgs e)
        {
            PlayMusic(false, "mainMenuMusic");

            GameplayScreen gameplayScreen = new GameplayScreen();
            var oldScreen = GetGameplayScreen();

            gameplayScreen.CurrentLevel = oldScreen.CurrentLevel;

            gameplayScreen.PlaySounds(false);

            string strInstruction = "instructionBackground_" + SettingsManager.Language;

            ReplaceAllScreens(
                new List<GameScreen>() 
                { 
                    new BackgroundScreen(strInstruction),
                    new LoadingScreen(gameplayScreen) 
                });

            TestButtons("Restart");
        }

        void ExitEntrySelected(object sender, EventArgs e)
        {
            AudioManager.StopSounds();

            ReplaceAllScreens(
                new List<GameScreen>() 
                { 
                    new BackgroundScreen("titleScreen"),
                    new MainMenuScreen()
                });

            TestButtons("Exit");
        }

        void TestButtons(string text)
        {
            Debug.WriteLine(String.Format("Pressed {0}", text));
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

        private void ContinueGame()
        {
            // Resume sounds and activate the gameplay screen
            PlayMusic(false, "mainMenuMusic");

            var screens = ScreenManager.GetScreens();

            foreach (GameScreen screen in screens)
            {
                if (!(screen is GameplayScreen))
                {
                    screen.ExitScreen();
                }
            }

            var gmpScreen = (ScreenManager.GetScreens()[0] as GameplayScreen);

            gmpScreen.PauseResumeSounds(true);

            gmpScreen.IsActive = true;
        }

        protected override void OnCancel()
        {
            ContinueGame();
        }

        #endregion
    }
}
