using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Linq;

namespace ZoneGame
{
    public static class SettingsManager
    {
        #region Fields

        //private static SettingsData settingsData;

        private static int maxLevel = LoadMaxLevel();
        private static Language language;

        #endregion

        #region Propertis
        /*
        public static SettingsData SettingsData
        {
            get { return settingsData; }
            set { settingsData = value; }
        }
        
        public static bool SoundEnabled
        {
            get { return settingsData.soundEnabled; }
            set { settingsData.soundEnabled = value; }
        }
        */
        public static int MaxLevel
        {
            get { return maxLevel; }
        }

        public static Language Language
        {
            get { return language; }
            set { language = value; }
        }

        #endregion        

        private static int LoadMaxLevel()
        {
            // Load Characters definitions
            XDocument worldDoc = XDocument.Load("Content/Documents/WorldDefinitions.xml");

            if (worldDoc.Element("Levels").Descendants("Level") != null)
            {
                return worldDoc.Element("Levels").Elements("Level").ToArray().Length;
            }

            return 0;
        }
    }
}
