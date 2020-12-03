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

namespace ZoneGame
{
    class MainMenuScreen : BasicMenu
    {
        #region Fields

        string [] names = new string []{"Start", "Continue", "Options", "Highscore", "Credits"};

        List<TextButton> textButtons = new List<TextButton>();

        #endregion

        #region Initialization

        public override void LoadContent()
        {
            base.LoadContent();

            LanguageDefinitions = Reader.LoadLanguage("MainMenu");

            title.TextContents = LanguageDefinitions["Title"];

            InitializeButtons();

            if (!AudioManager.IsActiveSong("mainMenuMusic"))
            {
                PlayMusic(true, "mainMenuMusic");
            }
        }

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
                    case "Start":
                        textButtons[i].Selected += StartEntrySelected;
                        break;
                    case "Continue":
                        textButtons[i].Selected += ContinueEntrySelected;
                        break;
                    case "Options":
                        textButtons[i].Selected += OptionsEntrySelected;
                        break;                    
                    case "Highscore":
                        textButtons[i].Selected += HighscoreEntrySelected;
                        break;
                    case "Credits":
                        textButtons[i].Selected += CreditsEntrySelected;
                        break;
                }

                if (names[i] == "Continue")
                {
                    /*
                    if (!MemoryStateManager.GameDataFileExists())
                    {
                        continue;
                    }*/
                    if (!PhoneApplicationService.Current.State.ContainsKey("CurrentLevel"))
                        continue;

                }

                AddVerticalTextButton(textButtons[i]);
            }
        }

        void StartEntrySelected(object sender, EventArgs e)
        {
            /*
            GameplayScreen gameplayScreen = new GameplayScreen();
            ReplaceForwardScreens(new List<GameScreen> (){new LevelScreen(gameplayScreen)});
            */
            if (SettingsManager.MaxLevel > 0)
            {
                string strInstruction = "instructionBackground_" + SettingsManager.Language;

                GameplayScreen gameplayScreen = new GameplayScreen();
                ReplaceForwardScreens(new List<GameScreen>() { new BackgroundScreen(strInstruction), new LoadingScreen(gameplayScreen) });
            }

            PlayMusic(false, "mainMenuMusic");

            TestButtons("Start");
        }

        void ContinueEntrySelected(object sender, EventArgs e)
        {
            if (SettingsManager.MaxLevel > 0)
            {
                GameplayScreen gameplayScreen = new GameplayScreen();
                if (PhoneApplicationService.Current.State.ContainsKey("CurrentLevel"))
                    gameplayScreen.CurrentLevel = int.Parse(PhoneApplicationService.Current.State["CurrentLevel"].ToString());

                string strInstruction = "instructionBackground_" + SettingsManager.Language;
                ReplaceForwardScreens(new List<GameScreen>() { new BackgroundScreen(strInstruction), new LoadingScreen(gameplayScreen) });
            }

            PlayMusic(false, "mainMenuMusic");

            TestButtons("Start");
        }

        void OptionsEntrySelected(object sender, EventArgs e)
        {
            ReplaceForwardScreens(new List<GameScreen>() { new OptionsScreen() });
            //TestButtons("Options");
        }

        void HelpEntrySelected(object sender, EventArgs e)
        {
            //ReplaceForwardScreens(new List<GameScreen>() { new HelpScreen() });
            TestButtons("Help");
        }

        void HighscoreEntrySelected(object sender, EventArgs e)
        {
            ReplaceForwardScreens(new List<GameScreen>() { new HighScoreScreen() });
            TestButtons("Highscore");
        }

        void CreditsEntrySelected(object sender, EventArgs e)
        {
            ReplaceAllScreens(new List<GameScreen>() { new CreditsScreen() });

            PlayMusic(false, "mainMenuMusic");
            //TestButtons("Credits");
        }

        void TestButtons(string text)
        {
            Debug.WriteLine(String.Format("Pressed {0}", text));
        }

        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();

            PlayMusic(false, "mainMenuMusic");
        }

        #endregion
    }
}
