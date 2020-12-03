using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace ZoneGame
{
    class HighScoreScreen : MenuScreen
    {
        #region Fields

        #region Score Fields

        private const string HighScoreDataDestination = "HighscoreData.sav";

        private static int currentLevel = 0;
        private static int totLevels = SettingsManager.MaxLevel;

        const int highscorePlaces = 5;

        //List<KeyValuePair<string,string>> Scores = new List<KeyValuePair<string,int>>();
        public static List<KeyValuePair<string, int>>[] highScore = InitializeScore();


        Dictionary<int, string> numberPlaceMapping = new Dictionary<int, string>();

        #endregion

        #region Animation Fields

        Texture2D blankTexture, prevButtonTexture, nextButtonTexture, topScoreBarTexture, bottomScoreBarTexture, levelScoreBarTexture;

        SpriteFont menuFont, scoreFont, noticeFont;

        protected int scoreBarAnimationStep = 15;
        protected int levelBarAnimationStep = 10;
        protected int glassScreenAnimationStep = 20;

        Text title, level, page;
        Image topBar, bottomBar, levelBar;
        ImageSelectable prevButton, nextButton;

        Rectangle glassScreenDimension = new Rectangle(0, 0, 0, 0);

        protected float alphaChannel = 0.8f;

        Vector2 scoreBlockDistance = new Vector2(20, 10);
        Vector2 topBarStartedPosition;
        Vector2 topBarFinishedPosition;
        Vector2 bottomBarStartedPosition;
        Vector2 bottomBarFinishedPosition;
        Vector2 levelBarStartedPosition;
        Vector2 levelBarFinishedPosition;
        Vector2 glassScreenOpenedDimension;

        bool animateScoreBar;
        bool scoreBarInTransition;
        bool scoreBarHitFinalPosition = false;

        bool levelBarInTransition;
        bool levelBarHitFinalPosition = false;

        bool glassScreenInTransition;
        bool glassScreenHitFinalPosition = false;

        bool drawHighScore = true;

        TimeSpan highScoreDelay = TimeSpan.FromSeconds(0.2);
        TimeSpan elapsedTime;

        #endregion

        #endregion

        #region Initialization

        public HighScoreScreen()
        {
            EnabledGestures = GestureType.Tap;

            initializeMapping();
            //highScore = InitializeScore();
            elapsedTime = TimeSpan.Zero;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            LanguageDefinitions = Reader.LoadLanguage("Highscore");

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 screenOrigin = new Vector2(viewport.Width / 2, viewport.Height / 2);

            // Carica texture
            blankTexture = contentDynamic.Load<Texture2D>("Textures/blank");
            topScoreBarTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/topScoreBar");
            bottomScoreBarTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/bottomScoreBar");
            levelScoreBarTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/levelScoreBar");
            nextButtonTexture = contentDynamic.Load<Texture2D>("Textures/Buttons/nextScoreButton");
            prevButtonTexture = contentDynamic.Load<Texture2D>("Textures/Buttons/prevScoreButton");
            
            // Carica font
            menuFont = contentDynamic.Load<SpriteFont>("Fonts/MenuFont");
            noticeFont = contentDynamic.Load<SpriteFont>("Fonts/noticeFont");
            scoreFont = contentDynamic.Load<SpriteFont>("Fonts/font14px");

            // Inizializza le posizioni iniziali e finali degli oggetti
            topBarStartedPosition = new Vector2
                (screenOrigin.X - topScoreBarTexture.Width / 2, 
                screenOrigin.Y - topScoreBarTexture.Height);
            
            topBarFinishedPosition = new Vector2(screenOrigin.X - topScoreBarTexture.Width / 2, 0);
            
            bottomBarStartedPosition = new Vector2(screenOrigin.X - topScoreBarTexture.Width / 2, screenOrigin.Y);
            
            bottomBarFinishedPosition = new Vector2(screenOrigin.X - bottomScoreBarTexture.Width / 2, viewport.Height - bottomScoreBarTexture.Height);            

            levelBarStartedPosition = new Vector2(
                topBarFinishedPosition.X + topScoreBarTexture.Width / 2 - levelScoreBarTexture.Width /2,
                topBarFinishedPosition.Y + topScoreBarTexture.Height - levelScoreBarTexture.Height);
            
            levelBarFinishedPosition = new Vector2(
                topBarFinishedPosition.X + topScoreBarTexture.Width / 2 - levelScoreBarTexture.Width / 2,
                topBarFinishedPosition.Y + topScoreBarTexture.Height);
            
            glassScreenOpenedDimension = new Vector2
                (topScoreBarTexture.Width - 30,
                bottomBarFinishedPosition.Y - (topBarFinishedPosition.Y + topScoreBarTexture.Height));

            glassScreenDimension = new Rectangle
                ((int)topBarStartedPosition.X + 15,
                (int)screenOrigin.Y + topScoreBarTexture.Height,
                topScoreBarTexture.Width - 30,
                0);

            // Crea title
            title = new Text(LanguageDefinitions["Title"], menuFont);
            title.Scale = 1.8f;
            title.Color = Color.Yellow;
            //Components.Add(title);
            // Crea level
            level = new Text("", scoreFont);
            //Components.Add(level);
            // Crea page
            page = new Text("", menuFont);
            //Components.Add(page);

            UpdateTextContent();

            //Crea oggetti
            topBar = new Image(topScoreBarTexture);
            bottomBar = new Image(bottomScoreBarTexture);
            levelBar = new Image(levelScoreBarTexture);
            prevButton = new ImageSelectable(prevButtonTexture);
            prevButton.Selected += OnPreviousButtonSelected;
            nextButton = new ImageSelectable(nextButtonTexture);
            nextButton.Selected += OnNextButtonSelected;
            Components.Add(prevButton);
            Components.Add(nextButton);

            topBar.Position = topBarStartedPosition;
            bottomBar.Position = bottomBarStartedPosition;
            levelBar.Position = levelBarStartedPosition;
            UpdateTextPosition();
            UpdateButtonPosition();

            this.animateScoreBar = true;

            if (animateScoreBar)
            {
                scoreBarInTransition = true;
            }
        }

        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {
            
            if(input == null)
                throw new ArgumentNullException("input");

            if (input.IsPauseGame())
            {
                Exit();
            }
            
            if (levelBarHitFinalPosition)
            {
                base.HandleInput(input);
            }
        }

        #endregion

        #region Update

        private void UpdateTextContent()
        {
            if (totLevels > 0)
            {
                level.TextContents = String.Format("{0} {1}", LanguageDefinitions["Level"], currentLevel + 1);
                page.TextContents = String.Format("{0}/{1}", currentLevel + 1, totLevels);
            }
            else
            {
                level.TextContents = String.Format("{0} {1}", LanguageDefinitions["Level"], 0);
                page.TextContents = String.Format("{0}/{1}", 0, 0);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (animateScoreBar)
            {
                if (scoreBarInTransition)
                {
                    AnimateScoreBar();
                }
                if (glassScreenInTransition)
                    AnimateGlassScreen();
                
                if (levelBarInTransition)
                    AnimateLevelBar();

                if (levelBarHitFinalPosition)
                {
                    //base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                    if (!drawHighScore)
                    {
                        elapsedTime += gameTime.ElapsedGameTime;
                        if (elapsedTime.TotalMilliseconds >= highScoreDelay.TotalMilliseconds)
                        {
                            drawHighScore = true;
                            elapsedTime = TimeSpan.Zero;
                        }
                    }
                }
                  
            }

            UpdateTextContent();

                /*
            else
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            }
                 * */
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            
            if (glassScreenInTransition)
            {
                // Draw the GlassScreen
                spriteBatch.Draw(blankTexture, glassScreenDimension, Color.Aqua * 0.6f);
            }

            if (levelBarInTransition)
            {
                levelBar.Draw(spriteBatch, gameTime);
            }

            // Draw the SideBar
            topBar.Draw(spriteBatch, gameTime);
            bottomBar.Draw(spriteBatch, gameTime);
            prevButton.Draw(spriteBatch, gameTime);
            nextButton.Draw(spriteBatch, gameTime);

            Vector2 debugPosition = Vector2.Zero;
            spriteBatch.DrawString(scoreFont, String.Format("glassScreen Height: {0}", glassScreenDimension.Height), debugPosition, Color.White);
            debugPosition.Y += 20;
            spriteBatch.DrawString(scoreFont, String.Format("glassScreenOpened Position: {0}", glassScreenOpenedDimension), debugPosition, Color.White);
            debugPosition.Y += 20;
            spriteBatch.DrawString(scoreFont, String.Format("bottomBar Position {0}", bottomBar.Position), debugPosition, Color.White);
            debugPosition.Y += 20;
            spriteBatch.DrawString(scoreFont, String.Format("level Hit: {0}", levelBarHitFinalPosition), debugPosition, Color.White);
            debugPosition.Y += 20;
            spriteBatch.DrawString(scoreFont, String.Format("current Level {0}", currentLevel), debugPosition, Color.White);
            debugPosition.Y += 20;
            spriteBatch.DrawString(scoreFont, String.Format("elapsed time {0}", elapsedTime), debugPosition, Color.White);
            debugPosition.Y += 20;
            spriteBatch.DrawString(scoreFont, String.Format("delay time {0}", highScoreDelay), debugPosition, Color.White);
            debugPosition.Y += 20;
            spriteBatch.DrawString(scoreFont, String.Format("elapsed totTime: Sec:{0} Mill:{1}", elapsedTime.TotalSeconds, elapsedTime.TotalMilliseconds), debugPosition, Color.White);
            debugPosition.Y += 20;
            spriteBatch.DrawString(scoreFont, String.Format("delay totTime: Sec:{0} Mill:{1}", highScoreDelay.TotalSeconds, highScoreDelay.TotalMilliseconds), debugPosition, Color.White);

            if (levelBarHitFinalPosition)
            {
                if (drawHighScore)
                {
                    DrawHighScore(spriteBatch);
                }
                title.Draw(spriteBatch, gameTime);
                page.Draw(spriteBatch, gameTime);
                level.Draw(spriteBatch, gameTime);
            }

            spriteBatch.End();
        }

        #endregion

        #region Private Methods

        private void AnimateScoreBar()
        {
            if (!scoreBarHitFinalPosition)
            {
                Vector2 pos = topBar.Position;
                pos.Y = MathHelper.Clamp(
                   topBar.Position.Y - scoreBarAnimationStep,
                   topBarFinishedPosition.Y,
                   topBarStartedPosition.Y);
                topBar.Position = pos;

                pos = bottomBar.Position;
                pos.Y = MathHelper.Clamp(
                    bottomBar.Position.Y + scoreBarAnimationStep,
                    bottomBarStartedPosition.Y,
                    bottomBarFinishedPosition.Y);
                bottomBar.Position = pos;

                UpdateButtonPosition();

                if (topBar.Position == topBarFinishedPosition &&
                    bottomBar.Position == bottomBarFinishedPosition)
                {
                    if (!scoreBarHitFinalPosition)
                    {
                        scoreBarHitFinalPosition = true;
                        glassScreenInTransition = true;
                    }
                    else
                    {
                        scoreBarHitFinalPosition = false;
                    }
                }
            }
        }

        private void AnimateLevelBar()
        {
            if (!levelBarHitFinalPosition)
            {
                Vector2 pos = levelBar.Position;
                pos.Y = MathHelper.Clamp(
                   levelBar.Position.Y + levelBarAnimationStep,
                   levelBarStartedPosition.Y,
                   levelBarFinishedPosition.Y);
                levelBar.Position = pos;

                if (levelBar.Position == levelBarFinishedPosition)
                {
                    if (!levelBarHitFinalPosition)
                    {
                        levelBarHitFinalPosition = true;
                    }
                    else
                    {
                        levelBarHitFinalPosition = false;
                    }
                }
            }
        }

        private void AnimateGlassScreen()
        {
            if (!glassScreenHitFinalPosition)
            {
                glassScreenDimension = new Rectangle((int)glassScreenDimension.X,
                (int)topBar.Position.Y + topBar.Height(),
                glassScreenDimension.Width,
                (int)MathHelper.Clamp(
                    glassScreenDimension.Height + glassScreenAnimationStep,
                    0f,
                    glassScreenOpenedDimension.Y));
                if (glassScreenDimension.Height == (int)glassScreenOpenedDimension.Y)
                {
                    if (!glassScreenHitFinalPosition)
                    {
                        glassScreenHitFinalPosition = true;
                        levelBarInTransition = true;
                    }
                    else
                    {
                        glassScreenHitFinalPosition = false;
                    }
                }
            }
        }
        private void UpdateTextPosition()
        {
            title.Position = new Vector2
            (topBarFinishedPosition.X + topScoreBarTexture.Width / 2 - title.Width() / 2,
            topBarFinishedPosition.Y + topScoreBarTexture.Height / 2 - title.Height() / 2);
            level.Position = new Vector2
            (levelBarFinishedPosition.X + levelScoreBarTexture.Width / 2 - level.Width() / 2,
            levelBarFinishedPosition.Y + levelScoreBarTexture.Height / 2 - level.Height() / 2);
            page.Position = new Vector2
            (bottomBarFinishedPosition.X + bottomScoreBarTexture.Width / 2 - page.Width() / 2,
                bottomBarFinishedPosition.Y + bottomScoreBarTexture.Height / 2 - page.Height() / 2);
        }

        private void UpdateButtonPosition()
        {
            prevButton.Position = new Vector2
                (bottomBar.Position.X + bottomBar.ImageContents.Width / 8,
                bottomBar.Position.Y + bottomBar.ImageContents.Height / 2 - prevButton.ImageContents.Height / 2);
            nextButton.Position = new Vector2
                (bottomBar.Position.X + bottomBar.ImageContents.Width - bottomBar.ImageContents.Width / 8 - nextButton.ImageContents.Width,
                bottomBar.Position.Y + bottomBar.ImageContents.Height / 2 - prevButton.ImageContents.Height / 2);
        }

        private void DrawHighScore(SpriteBatch spriteBatch)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            // Draw the highscores table
            float height = glassScreenOpenedDimension.Y - levelBarFinishedPosition.Y + levelScoreBarTexture.Height - 20;
            Vector2 pos = new Vector2(glassScreenDimension.X, levelBarFinishedPosition.Y + levelScoreBarTexture.Height + 20);


            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            string noName = "/";
            int noScore = 0;
            Vector2 sizeNoName = scoreFont.MeasureString(noName);
            Vector2 sizeNoScore = scoreFont.MeasureString(noScore.ToString());

            if (highScore != null)
            {
                if (highScore.Length > 0)
                {
                    list.AddRange(highScore[currentLevel]);
                    if (highScore[currentLevel].Count < highscorePlaces)
                    {
                        for (int j = highScore[currentLevel].Count; j < highscorePlaces; j++)
                        {
                            list.Add(new KeyValuePair<string, int>(noName, noScore));
                        }
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        // Draw place number
                        Vector2 dinPos = pos + new Vector2(0, i * (height / highscorePlaces));

                        // Draw place number
                        spriteBatch.DrawString(scoreFont, GetPlaceString(i),
                            new Vector2(dinPos.X + 40 - scoreFont.MeasureString(GetPlaceString(i)).X / 2, dinPos.Y), Color.Black);

                        if (list[i].Key != null)
                        {
                            // Draw Name
                            ScreenManager.SpriteBatch.DrawString
                                (scoreFont, highScore[currentLevel][i].Key,
                                new Vector2(dinPos.X + glassScreenDimension.Width / 2 - scoreFont.MeasureString(highScore[currentLevel][i].Key).X / 2,
                                dinPos.Y), Color.DarkRed);
                        }
                        else
                        {
                            // Draw Name
                            ScreenManager.SpriteBatch.DrawString
                                (scoreFont, noScore.ToString(),
                                new Vector2(dinPos.X + glassScreenDimension.Width / 2 - sizeNoName.X / 2,
                                dinPos.Y), Color.DarkRed);
                        }
                        // Draw score
                        ScreenManager.SpriteBatch.DrawString(scoreFont, highScore[currentLevel][i].Value.ToString(),
                            new Vector2(dinPos.X + glassScreenDimension.Width - 40 - scoreFont.MeasureString(highScore[currentLevel][i].Value.ToString()).X / 2, dinPos.Y), Color.Yellow);
                    }
                }
                else
                {
                    Vector2 strSize = menuFont.MeasureString(LanguageDefinitions["NoScore"]);

                    spriteBatch.DrawString(
                        menuFont,
                        LanguageDefinitions["NoScore"],
                        new Vector2((viewport.Width - strSize.X) / 2, (viewport.Height - strSize.Y) / 2),
                        Color.Red);
                }
            }
            else
            {
                Vector2 strSize = menuFont.MeasureString(LanguageDefinitions["NoScore"]);

                spriteBatch.DrawString(
                    menuFont,
                    LanguageDefinitions["NoScore"],
                    new Vector2((viewport.Width - strSize.X) / 2, (viewport.Height - strSize.Y) / 2),
                    Color.Red);
            }
        }

        private void Exit()
        {
            this.ExitScreen();
            ScreenManager.AddScreen(new MainMenuScreen());
        }

        #endregion

        #region Highscore loading/saving logic

        /// <summary>
        /// Check if a score belongs on the high score table.
        /// </summary>
        /// <returns></returns>
        public static bool IsInHighscores(int score, int level)
        {
            // If the score is better than the worst score in the table    
                return score > highScore[level][highscorePlaces - 1].Value;
            
        }

        /// <summary>
        /// Put high score on highscores table.
        /// </summary>
        /// <param name="name">Player's name.</param>
        /// <param name="score">The player's score.</param>
        public static void PutHighScore(string playerName, int score, int level)
        {
            if (highScore.Length >= level + 1)
            {
                if (IsInHighscores(score, level))
                {
                    highScore[level][highscorePlaces - 1] = new KeyValuePair<string, int>(playerName, score);
                    OrderGameScore(level);
                    SaveHighscore();
                }
            }
        }

        /// <summary>
        /// Order the high scores table.
        /// </summary>
        private static void OrderGameScore(int level)
        {
            highScore[level].Sort(CompareScores);
        }

        /// <summary>
        /// Comparison method used to compare two highscore entries.
        /// </summary>
        /// <param name="score1">First highscore entry.</param>
        /// <param name="score2">Second highscore entry.</param>
        /// <returns>1 if the first highscore is smaller than the second, 0 if both
        /// are equal and -1 otherwise.</returns>
        private static int CompareScores(KeyValuePair<string, int> score1,
            KeyValuePair<string, int> score2)
        {
            if (score1.Value < score2.Value)
            {
                return 1;
            }

            if (score1.Value == score2.Value)
            {
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// Saves the current highscore to a text file. 
        /// </summary>
        public static void SaveHighscore()
        {
            // Get the place to store the data
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists(HighScoreDataDestination))
                {
                    isf.DeleteFile(HighScoreDataDestination);
                }
                // Create the file to save the data
                using (IsolatedStorageFileStream isfs = isf.CreateFile(HighScoreScreen.HighScoreDataDestination))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                    //serializer.Serialize(isfs, highScore);
                    isfs.Close();
                    isf.Dispose();
                    /*using (StreamWriter writer = new StreamWriter(isfs))
                    {
                        for (int i = 0; i < highScore.Count; i++)
                        {
                            for (int j = 0; j < highScore[i].Count; j++)
                            {
                                // Write the scores
                                //writer.WriteLine(highScore[i][j].Key);
                                //writer.WriteLine(highScore[i][j].Value.ToString());
                            }
                        }
                    }*/
                }
            }
        }

        /// <summary>
        /// Loads the high score from a text file.  
        /// </summary>
        public static void LoadHighscores()
        {
            // Get the place the data stored
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Try to open the file
                if (isf.FileExists(HighScoreScreen.HighScoreDataDestination))
                {
                    using (IsolatedStorageFileStream isfs =
                        isf.OpenFile(HighScoreScreen.HighScoreDataDestination, FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
                        //highScore = (List<KeyValuePair<string, int>> [])serializer.Deserialize(isfs);
                        isfs.Close();
                        isf.Dispose();
                        /*
                        // Get the stream to read the data
                        using (StreamReader reader = new StreamReader(isfs))
                        {
                            // Read the highscores
                            int i = 0;
                            while (!reader.EndOfStream)
                            {
                                string name = reader.ReadLine();
                                string score = reader.ReadLine();
                                highScore[i++] = new KeyValuePair<string, int>(name, int.Parse(score));
                            }
                        }
                         */
                    }
                }
            }
        }

        private string GetPlaceString(int number)
        {
            return numberPlaceMapping[number];
        }

        private void initializeMapping()
        {
            numberPlaceMapping.Add(0, "1ST");
            numberPlaceMapping.Add(1, "2ND");
            numberPlaceMapping.Add(2, "3RD");
            numberPlaceMapping.Add(3, "4TH");
            numberPlaceMapping.Add(4, "5TH");
        }

        private void OnPreviousButtonSelected(object sender, EventArgs e)
        {
            if (currentLevel > 0)
            {
                currentLevel--;
                //drawHighScore = false;
            }
        }

        private void OnNextButtonSelected(object sender, EventArgs e)
        {
            if (currentLevel < totLevels - 1)
            {
                currentLevel++;
                //drawHighScore = false;
            }
        }

        private static List<KeyValuePair<string,int>> [] InitializeScore()
        {
            List<KeyValuePair<string,int>> [] list = new List<KeyValuePair<string,int>> [totLevels];
            string noName = "/";
            int noScore = 0;

            for (int i = 0; i < list.Length; i++)
            {
                list[i] = new List<KeyValuePair<string, int>>();
                for (int j = 0; j < highscorePlaces; j++)
                {
                    list[i].Add(new KeyValuePair<string, int>(noName, noScore));
                }
            }

            return list;
        }

        #endregion
    }
}
