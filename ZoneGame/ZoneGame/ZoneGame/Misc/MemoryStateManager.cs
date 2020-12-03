using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Phone.Shell;

namespace ZoneGame
{

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
    }

    public enum Language
    {
        eng,
        ita,
    }

    public struct GameData
    {
        public int currentLevel;
    }

    public struct SettingsData
    {
        public bool soundEnabled;
        public Language language;
    }

    public static class MemoryStateManager
    {

        #region Fields 

        const string GameDataDestination = "GameData.sav";
        const string SettingsDataDestination = "SettingsData.sav";

        #endregion

        #region Static Methods

        public static bool GameDataFileExists()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (storage.FileExists(GameDataDestination))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SettingsDataFileExists()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (storage.FileExists(SettingsDataDestination))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static GameData LoadGameData()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            GameData gamedata = new GameData();

            if (storage.FileExists(GameDataDestination))
            {
                IsolatedStorageFileStream stream = storage.OpenFile(GameDataDestination, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                gamedata = (GameData)serializer.Deserialize(stream);
                stream.Close();
                stream.Dispose();
                PhoneApplicationService.Current.State["CurrentLevel"] = gamedata.currentLevel;
            }
            else
            {
                storage.Dispose();
                gamedata = new GameData
                {
                    currentLevel = 0,
                };
            }

            return gamedata;
        }

        public static void SaveGameData(GameData gamedata)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.FileExists(GameDataDestination))
            {
                storage.DeleteFile(GameDataDestination);
            }
            IsolatedStorageFileStream stream = storage.CreateFile(GameDataDestination);
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            serializer.Serialize(stream, gamedata);
            stream.Close();
            storage.Dispose();
        }

        public static SettingsData LoadSettingsData()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            SettingsData settingsdata = new SettingsData();

            if (storage.FileExists(SettingsDataDestination))
            {
                IsolatedStorageFileStream stream = storage.OpenFile(SettingsDataDestination, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
                settingsdata = (SettingsData)serializer.Deserialize(stream);
                stream.Close();
                storage.Dispose();
            }
            else
            {
                settingsdata = new SettingsData
                {
                    soundEnabled = true,
                    language = Language.eng,
                };
                storage.Dispose();
            }

            return settingsdata;
        }

        public static void SaveSettingsData(SettingsData settingsdata)
        {
            
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.FileExists(SettingsDataDestination))
            {
                storage.DeleteFile(SettingsDataDestination);
            }
            IsolatedStorageFileStream stream = storage.CreateFile(SettingsDataDestination);
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
            serializer.Serialize(stream, settingsdata);
            stream.Close();
            storage.Dispose();
        }

        public static void SaveCurrentGameState(int currentLevel)
        {
            PhoneApplicationService.Current.State["CurrentLevel"]
                = currentLevel;
        }

        #endregion
    }
}
