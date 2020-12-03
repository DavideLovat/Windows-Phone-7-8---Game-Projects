using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ZoneGame
{
    class OptionsScreen : BasicMenu
    {
        #region Fields
        
        Dictionary<Language, String> languages = new Dictionary<Language, string>() { { Language.ita, "Ita" }, { Language.eng, "Eng" } };

        TextButton SoundEntry, LanguageEntry;

        #endregion

        #region Initialization

        public override void LoadContent()
        {
            base.LoadContent();

            this.LanguageDefinitions = Reader.LoadLanguage("Options");

            InitializeButtons();

            SetEntryText();

            
        }

        #endregion

        #region Methods

        void SoundEntrySelected(object sender, EventArgs e)
        {
            /*
            SettingsManager.SoundEnabled = !SettingsManager.SoundEnabled;
            AudioManager.StopMusic();
            AudioManager.StopSounds();
             * */
            AudioManager.Enable(!AudioManager.IsActive) ;
            //AudioManager.PlayMusic("");
            SetEntryText();
        }

        void LanguageEntrySelected(object sender, EventArgs e)
        {
            switch (SettingsManager.Language)
            {
                case Language.eng:
                    {
                        SettingsManager.Language = Language.ita;
                        break;
                    }
                case Language.ita:
                    {
                        SettingsManager.Language = Language.eng;
                        break;
                    }
            }

            this.LanguageDefinitions = Reader.LoadLanguage("Options");

            SetEntryText();
        }

        private void InitializeButtons()
        {
            SoundEntry = new TextButton("", menuFont, buttonMenuTexture);
            LanguageEntry = new TextButton("", menuFont, buttonMenuTexture);

            SoundEntry.Selected += SoundEntrySelected;
            LanguageEntry.Selected += LanguageEntrySelected;

            AddVerticalTextButton(SoundEntry);
            AddVerticalTextButton(LanguageEntry);
        }

        void SetEntryText()
        {
            SoundEntry.TextContents = LanguageDefinitions["Sound"].ToString() + ": " + (AudioManager.IsActive ? "on" : "off");
            LanguageEntry.TextContents = LanguageDefinitions["Language"].ToString() + ": " + languages[SettingsManager.Language];
            title.TextContents = LanguageDefinitions["Title"].ToString();
        }

        protected override void OnCancel()
        {
            ReplaceForwardScreens(new List<GameScreen>() { new MainMenuScreen() });
        }

        #endregion
    }
}
