using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;


namespace QuizTime
{
    class MainMenuScreen : BasicMenu
    {
        #region Fields 

        ImageSelectable titlePlayButton;
        ImageSelectable titleHighscoreButton;

        #endregion

        #region Initialization

        public override void LoadContent()
        {
            base.LoadContent();

            AudioManager.LoadSounds();
            AudioManager.LoadMusic();

            AudioManager.PlaySound("gate");

            //AudioManager.PlayMusic("MenuMusic_Loop");            

            titlePlayButton = new ImageSelectable(contentDynamic.Load<Texture2D>("Textures/Buttons/titleButton_Play"));
            titleHighscoreButton = new ImageSelectable(contentDynamic.Load<Texture2D>("Textures/Buttons/titleButton_Highscore"));

            titlePlayButton.Selected += PlayButtonSelected;
            titleHighscoreButton.Selected += HighscoreButtonSelected;

            AddSideBarComponent(titlePlayButton);
            AddSideBarComponent(titleHighscoreButton);
        }

        #endregion

        #region Private Methods

        void PlayButtonSelected(object sender, EventArgs e)
        {
            /*
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            ScreenManager.AddScreen(new BackgroundScreen("Instructions"));
            ScreenManager.AddScreen(new LoadingAndInstructionScreen());

            AudioManager.StopSound("MenuMusic_Loop");
            */

            ReplaceAllScreens(new List<GameScreen>() { new LoadingScreen()});

            StopSounds();

            TestButtons("Start");
        }

        void HighscoreButtonSelected(object sender, EventArgs e)
        {
            ReplaceAllScreens(
                new List<GameScreen>()
                {
                    new BackgroundScreen("highscoreBackground"),
                    new HighScoreScreen()
                });

            StopSounds();

            TestButtons("High Score");
        }

        void TestButtons(string text)
        {
            Debug.WriteLine(String.Format("Pressed {0}", text));
        }

        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();

            StopSounds();
        }

        private void StopSounds()
        {
            AudioManager.StopSound("gate");
        }

        #endregion
    }
}
