using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ZoneGame
{
    public static class Reader
    {
        #region Fields

        const string LanguageDocPath = "Content/Documents/Language.xml";
        const string LevelDocPath = "Content/Documents/Level.xml";
        const string HelpReferencesDocPath = "Content/Documents/HelpReferences.xml";
        #endregion

        public static Dictionary<string, string> LoadLanguage(string sceneTag)
        {
            Dictionary<string,string> Language = new Dictionary<string, string>();
            XDocument languageDoc = XDocument.Load(LanguageDocPath);

            foreach (XElement scene in languageDoc.Document.Element("Scenes").Elements("Scene"))
            {
                if ((string)scene.Attribute("ID").Value == sceneTag)
                {
                    foreach (XElement text in scene.Elements("Text"))
                    {
                        Language.Add(text.Attribute("ID").Value, text.Element(SettingsManager.Language.ToString()).Value);
                    }
                    break;
                }
                continue;
            }

            return Language;
        }
        /*
        public static HelpContent LoadHelpReferences(int currentPage)
        {
            XDocument languageDoc = XDocument.Load(HelpReferencesDocPath);
            HelpContent references = new HelpContent();

            foreach (XElement page in languageDoc.Document.Elements("Pages").Elements("Page"))
            {
                if ((string)page.Attribute("ID").Value == currentPage.ToString())
                {
                    if(page.Element("ImagePath") != null)
                        references.image = page.Element("ImagePath").Value;
                    if(page.Element("Text") != null)
                        references.text = page.Element("Text").Element(SettingsManager.Language.ToString()).Value;
                    break;
                }
                continue;
            }

            return references;
        }
        */
    }

}
