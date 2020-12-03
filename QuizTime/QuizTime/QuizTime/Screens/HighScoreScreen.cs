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

namespace QuizTime
{
    class HighScoreScreen : MenuScreen
    {
        #region Fields Doors Animation

        Texture2D leftDoorTexture, rightDoorTexture;

        Image leftDoor, rightDoor;

        int doorsAnimationStep = 4;

        bool animateDoors;
        bool doorsInTransition;
        bool doorsHitFinalPosition = false;

        Vector2 leftDoorOpenedPosition;
        Vector2 leftDoorClosedPosition;
        Vector2 rightDoorOpenedPosition;
        Vector2 rightDoorClosedPosition;

        #endregion

        #region Fields
        
        private const string HighScoreDataDestination = "HighscoreData.sav";

        Texture2D blankTexture;

        SpriteFont menuFont, scoreFont;
        
        const int highscorePlaces = 6;

        //List<KeyValuePair<string,string>> Scores = new List<KeyValuePair<string,int>>();
        public static List<KeyValuePair<string, int>> highScore = new List<KeyValuePair<string,int>>();        

        bool drawHighScore = true;

        Dictionary<int, string> numberPlaceMapping = new Dictionary<int,string>();

        #endregion

        #region Initialization

        public HighScoreScreen()
        {
            EnabledGestures = GestureType.Tap;

            initializeMapping();

            animateDoors = true;

            //highScore = InitializeScore();

            if (animateDoors)
            {
                AudioManager.PlaySound("doorOpen");
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            // Load highscore
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 screenOrigin = new Vector2(viewport.Width / 2, viewport.Height / 2);

            // Carica texture
            blankTexture = contentDynamic.Load<Texture2D>("Textures/blank");
           
            // Carica font
            menuFont = contentDynamic.Load<SpriteFont>("Fonts/MenuFont");
            scoreFont = contentDynamic.Load<SpriteFont>("Fonts/font14px");

            // Crea title
            /*
            title = new Text(LanguageDefinitions["Title"], menuFont);
            title.Scale = 1.8f;
            title.Color = Color.Yellow;
            */

            // Load Animation Fields
            leftDoorTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/left_door");
            rightDoorTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/right_door");

            if (animateDoors)
                doorsInTransition = true;

            leftDoor = new Image(leftDoorTexture);
            rightDoor = new Image(rightDoorTexture);

            leftDoorOpenedPosition = new Vector2(
                -leftDoor.Width(this),
                viewport.Height - leftDoor.Height(this));
            leftDoorClosedPosition = new Vector2(
                0,
                viewport.Height - leftDoor.Height(this));

            rightDoorOpenedPosition = new Vector2(
                viewport.Width, viewport.Height - rightDoor.Height(this));
            rightDoorClosedPosition = new Vector2(
                viewport.Width - rightDoor.Width(this),
                viewport.Height - rightDoor.Height(this));
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
            
            if (input.Gestures.Count > 0)
            {
                GestureSample sample = input.Gestures[0];
                if (sample.GestureType == GestureType.Tap)
                {
                    Exit();

                    input.Gestures.Clear();
                }
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen)
        {
            base.Update(gameTime, false, false);

            if (doorsInTransition && animateDoors)
                AnimateDoors();    
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            int fraction = viewport.Width / 4;

            // Draw the title
            string title = "High Scores";
            Vector2 titleSize = scoreFont.MeasureString(title);
            Vector2 titlePosition = new Vector2(
                viewport.Width / 2 - titleSize.X / 2,
                320);
            Vector2 [] fractions = 
            {
                new Vector2(fraction, titlePosition.Y + 50 ),
                new Vector2(fraction * 2, titlePosition.Y + 50 ),
                new Vector2(fraction * 3, titlePosition.Y + 50 ),
            };

            string po = "Postion";
            string pl = "Player";
            string an = "Answer";
            Vector2 poSize = scoreFont.MeasureString(po);
            Vector2 plSize = scoreFont.MeasureString(pl);
            Vector2 anSize = scoreFont.MeasureString(an);
            Vector2 poPos = fractions[0] - new Vector2(poSize.X / 2,0);
            Vector2 plPos = fractions[1] - new Vector2(plSize.X / 2, 0); ;
            Vector2 anPos = fractions[2] - new Vector2(anSize.X / 2, 0); ;

            spriteBatch.Begin();

            spriteBatch.DrawString(scoreFont, title, titlePosition, Color.Red);
            spriteBatch.DrawString(scoreFont, po, poPos, Color.Red);
            spriteBatch.DrawString(scoreFont, pl, plPos, Color.Red);
            spriteBatch.DrawString(scoreFont, an, anPos, Color.Red);

            float posY = titlePosition.Y + 80;


            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            string noName = "/";
            int noScore = 0;

            if (highScore.Count > 0)
                list.AddRange(highScore);

            if (highScore.Count < highscorePlaces)
            {
                for (int j = highScore.Count; j < highscorePlaces; j++)
                {
                    list.Add(new KeyValuePair<string, int>(noName, noScore));
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (!string.IsNullOrEmpty(list[i].Key))
                {
                    Vector2 size = Vector2.Zero;

                    // Draw place number
                    size = scoreFont.MeasureString(GetPlaceString(i));
                    ScreenManager.SpriteBatch.DrawString(scoreFont, GetPlaceString(i),
                        new Vector2(fractions[0].X - size.X / 2, i * 72 + posY), Color.YellowGreen);

                    // Draw Name
                    size = scoreFont.MeasureString(list[i].Key);
                    ScreenManager.SpriteBatch.DrawString(scoreFont, list[i].Key,
                        new Vector2(fractions[1].X - size.X / 2, i * 72 + posY), Color.DarkRed);

                    // Draw score
                    size = scoreFont.MeasureString(list[i].Value.ToString());
                    ScreenManager.SpriteBatch.DrawString(scoreFont, list[i].Value.ToString(),
                        new Vector2(fractions[2].X - size.X / 2, i * 72 + posY), Color.Yellow);
                }  
            }

            // Draw the doors
            spriteBatch.Draw(leftDoor.ImageContents, leftDoor.Position, Color.White);
            spriteBatch.Draw(rightDoor.ImageContents, rightDoor.Position, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion


        #region Private Methods
        
        private void Exit()
        {
            AudioManager.StopSound("doorOpen");

            ReplaceAllScreens(
                new List<GameScreen>() 
                { 
                    new BackgroundScreen("titleBackground"),
                    new MainMenuScreen() 
                });
        }

        private void AnimateDoors()
        {
            if (!doorsHitFinalPosition)
            {
                Vector2 pos = Vector2.Zero;

                pos = leftDoor.Position;
                pos.X = MathHelper.Clamp(
                    leftDoor.Position.X - doorsAnimationStep,
                    leftDoorOpenedPosition.X,
                    leftDoorClosedPosition.X);
                leftDoor.Position = pos;

                pos = rightDoor.Position;
                pos.X = MathHelper.Clamp(
                    pos.X + doorsAnimationStep,
                    rightDoorClosedPosition.X,
                    rightDoorOpenedPosition.X);
                rightDoor.Position = pos;

                if (leftDoor.Position == leftDoorOpenedPosition &&
                    rightDoor.Position == rightDoorOpenedPosition)
                {
                    if (!doorsHitFinalPosition)
                        doorsHitFinalPosition = true;
                    else
                        doorsHitFinalPosition = false;
                }
            }
        }

        #endregion

        #region Highscore loading/saving logic

        /// <summary>
        /// Check if a score belongs on the high score table.
        /// </summary>
        /// <returns></returns>
        public static bool IsInHighscores(int score)
        {
            // If the score is better than the worst score in the table  
            if (highScore.Count > 0 && highScore.Count == highscorePlaces)
                return score > highScore[highscorePlaces - 1].Value;
            else
                return true;
        }

        /// <summary>
        /// Put high score on highscores table.
        /// </summary>
        /// <param name="name">Player's name.</param>
        /// <param name="score">The player's score.</param>
        public static void PutHighScore(string playerName, int score)
        {
            if (IsInHighscores(score))
            {
                if (highScore.Count > 0 && highScore.Count == highscorePlaces )
                {
                    highScore[highscorePlaces - 1] = new KeyValuePair<string, int>(playerName, score);
                }
                else
                {
                    highScore.Add(new KeyValuePair<string, int>(playerName, score));
                }
                OrderGameScore();
                SaveHighscore();
            }
            
        }

        /// <summary>
        /// Order the high scores table.
        /// </summary>
        private static void OrderGameScore()
        {
            highScore.Sort(CompareScores);
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
            // Get the place to store data
            using (IsolatedStorageFile isf = 
                IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Create a file to save the highscore data
                
                using (IsolatedStorageFileStream isfs = 
                    isf.CreateFile("highscores.txt"))
                {
                    using (StreamWriter writer = new StreamWriter(isfs))
                    {
                        for (int i = 0; i < highScore.Count; i++)
                        {
                            // Write the scores
                            writer.WriteLine(highScore[i].Key);
                            writer.WriteLine(highScore[i].Value.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the high score from a text file.  
        /// </summary>
        public static void LoadHighscores()
        {
            // Get the place where data is stored
            using (IsolatedStorageFile isf =
                IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Try to open the file
                if (isf.FileExists("highscores.txt"))
                {
                    using (IsolatedStorageFileStream isfs =
                        isf.OpenFile("highscores.txt", FileMode.Open))
                    {
                        // Get the stream to read the data
                        using (StreamReader reader = new StreamReader(isfs))
                        {
                            // Read the highscores
                            int i = 0;
                            while (!reader.EndOfStream)
                            {
                                string name = reader.ReadLine();
                                int score = int.Parse(reader.ReadLine());

                                highScore.Add (new KeyValuePair<string, int>(
                                    name, score));
                            }
                        }
                    }
                }
            }

            OrderGameScore();
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
            numberPlaceMapping.Add(5, "6TH");
        }

        private List<KeyValuePair<string,int>> InitializeScore()
        {
            List<KeyValuePair<string,int>> list = new List<KeyValuePair<string,int>> ();
            string noName = "/";
            int noScore = 0;

            if (highScore.Count > 0)
                list.AddRange(highScore);

            if (highScore.Count < highscorePlaces)
            {
                for (int j = highScore.Count; j < highscorePlaces; j++)
                {
                    list.Add(new KeyValuePair<string, int>(noName, noScore));
                }
            }

            return list;
        }

        #endregion
    }
}
