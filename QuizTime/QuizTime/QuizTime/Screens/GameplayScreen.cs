using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Phone.Shell;

namespace QuizTime
{
    /*
    struct GameData
    {
        public List<List<Question>> queryList;
        public Question currentQuery;
        public int outerListIndex;
        public int innerListIndex;
        public int rightAnswers;
        public int currentQuestionNumber;
        public int maxQuestionNumber;
        public TimeSpan clockTime;
        public int numLife;
    }
    */
    struct Question
    {
        public string Q;
        //public List<KeyValuePair<int, string>> A;
        public List<string> A;
        //public int RA;
        public string RA;
    }

    class GameplayScreen : GameScreen
    {
        #region Fields      
        
        private bool isActive;

        public new bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        Random random = new Random();

        //GameData gameData;

        List<List<Question>> queryList = new List<List<Question>>();
        //Question currentQuery;
        int outerListIndex = 0;
        int innerListIndex = 0;
        //int currentRightAnswer;
        string currentRightAnswer;
        int maxQuestionsPerList = 20;        
        //List<int> questionId = new List<int>();

        // Game State Fields
        private bool isShowComponents;
        private bool userTapToStar;
        private bool isUserWon;
        private bool gameEnded;
        private bool userTapToExit;
        private bool hasAnswered;
        private bool insertInHighscore;
        private bool nextQuery;
        private bool isLifeLost;
        private bool isPlaySound;
        private bool isAlarmClock;

        bool moveToHighScore = false;

        int rightAnswers = 0;

        public int RightAnswers
        {
            get { return rightAnswers; }
        }

        int currentQuestionNumber = 0;
        int maxQuestionNumber;

        SpriteFont textFont;
        SpriteFont noticeFont;
        SpriteFont font16px;
        SpriteFont clockFont;

        Texture2D neutralTexture, successTexture, failureTexture;
        Texture2D backgroundTexture;
        Texture2D questionBlockTexture, innerQuestionBlockTexture, grillQuestionBlockTexture;
        Texture2D answersBlockTexture, innerAnswersBlockTexture, grillAnswersBlockTexture;
        Texture2D greenLightTexture, redLightTexture;
        Texture2D bottomBarTexture;

        // Game components
        QuestionBar questionBar;
        AnswerBar answerBar;
        LifeBar lifeBar;
        ClockBar clockBar;
        FlashingText flashingText;
        Text touchNotice;
        Paragraph questionParagraph;
        List<AnswerTextButton> answerButtons = new List<AnswerTextButton>();

        // Time fields
        TimeSpan timeToWait = TimeSpan.FromSeconds(1); 
        TimeSpan timeElapsed;
        TimeSpan flashingDuration = TimeSpan.FromSeconds(1);
        TimeSpan maxClockTime = TimeSpan.FromMinutes(5);

        bool isResuming;

        #endregion

        #region Properties

        private bool IsOpenComponentsBar
        {
            get
            {
                return
                    questionBar.IsOpenBar &&
                    answerBar.IsOpenBar &&
                    clockBar.IsOpenBar;
            }
        }

        private bool IsCloseComponentsBar
        {
            get
            {
                return
                    !questionBar.IsOpenBar &&
                    !answerBar.IsOpenBar &&
                    !clockBar.IsOpenBar;

            }
        }

        private bool ComponentsBarIsOpened
        {
            get 
            {
                return
                    questionBar.BarIsOpened &&
                    answerBar.BarIsOpened &&
                    clockBar.BarIsOpened; 
            }
        }

        private bool ComponentsBarIsClosed
        {
            get
            {
                return
                    !questionBar.BarIsOpened &&
                    !answerBar.BarIsOpened &&
                    !clockBar.BarIsOpened; 
            }
        }

        private bool IsOpenComponentsGrill
        {
            get
            {
                return
                    questionBar.IsOpenGrill &&
                    answerBar.IsOpenGrill;
            }
        }

        private bool ComponentsGrillIsOpened
        {
            get 
            {
                return
                    questionBar.GrillIsOpened &&
                    answerBar.GrillIsOpened;
            }
        }

        private bool ComponentsGrillIsClosed
        {
            get
            {
                return
                    !questionBar.GrillIsOpened &&
                    !answerBar.GrillIsOpened;
            }
        }

        #endregion

        #region Initializations

        public GameplayScreen()
        {
            EnabledGestures = GestureType.Tap;
        }

        #endregion


        #region Loading

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void LoadAssets()
        {
            if (contentDynamic == null)
                contentDynamic = new ContentManager(ScreenManager.Game.Services, "Content");
            
            LoadQuestionsFromXML();

            LoadTextures();

            CreateGameComponents();

            InitializeAnimationFields();

            //AudioManager.PlayMusic("InGameSong_Loop");
        }

        private void LoadQuestionsFromXML()
        {
            XDocument doc = XDocument.Load(@"Content\Documents\Questions.xml");
            var questionsElem = doc.Document.Descendants(XName.Get("Question"));

            int count = 0;
            List<Question> blankList = new List<Question>();
            List<XElement> questionsElemList = questionsElem.ToList<XElement>();
            //foreach (var question in questions)
            for(int i=0; i< questionsElemList.Count; i++)
            {
                var question = questionsElemList[i];
                Question q = new Question();
                q.Q = question.Element(XName.Get("Q")).Value;
                q.A = new List<string>();
                var answers = question.Elements(XName.Get("A"));
                foreach (var answer in answers)
                {
                    string text = answer.Value;
                    q.A.Add(text);
                }
                q.RA = question.Element(XName.Get("RA")).Value;

                blankList.Add(q);
                count++;

                if (count > maxQuestionsPerList || i == questionsElemList.Count - 1)
                {
                    if (blankList != null || blankList.Count > 0)
                        queryList.Add(blankList);
                    count = 0;
                }
                
                //questionId.Add(maxQuestionNumber);
                maxQuestionNumber++;
                
            }
        }

        private void LoadTextures()
        {
            textFont = contentDynamic.Load<SpriteFont>(@"Fonts\GameplaySmallFont");
            noticeFont = contentDynamic.Load<SpriteFont>(@"Fonts\GameplayLargeFont");
            font16px = contentDynamic.Load<SpriteFont>(@"Fonts\font16px");
            clockFont = contentDynamic.Load<SpriteFont>(@"Fonts\clockFont");
            //buttonMenu = contentDynamic.Load<Texture2D>(@"Textures\Buttons\buttonMenu");
            //buttonGameplay = contentDynamic.Load<Texture2D>(@"Textures\Buttons\buttonGameplay");
            neutralTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\answerCircleNeutral");;
            successTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\answerCircleSuccess"); ;
            failureTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\answerCircleFailure"); ;
            backgroundTexture = contentDynamic.Load<Texture2D>(@"Textures\Backgrounds\gameplayBackground");
            questionBlockTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\questionBlock");
            innerQuestionBlockTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\innerQuestionBlock");
            grillQuestionBlockTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\grillQuestionBlock");
            answersBlockTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\answersBlock");
            innerAnswersBlockTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\innerAnswersBlock");
            grillAnswersBlockTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\grillAnswersBlock");
            greenLightTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\greenLight");
            redLightTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\redLight");
            bottomBarTexture = contentDynamic.Load<Texture2D>(@"Textures\Gameplay\bottomBar");
        }

        private void CreateGameComponents()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport; 

            // Create question side bar
            Vector2 questionBlockClosedPosition = new Vector2(-questionBlockTexture.Width, 64);
            Vector2 questionBlockOpenedPosition = new Vector2(0, 64);

            questionBar = new QuestionBar(questionBlockTexture, innerQuestionBlockTexture, grillQuestionBlockTexture,
                questionBlockClosedPosition, questionBlockOpenedPosition, false, false);

            questionBar.innerSpace = new Vector2(31, 80);
            questionBar.paragraphSpace = new Vector2(10, 10);
            questionBar.BarAnimationStep = 15;
            questionBar.grillAnimationStep = 9;
            

            // Create answers side bar
            Vector2 answersBlockClosedPosition = new Vector2(viewport.Width, 485);
            Vector2 answersBlockOpenedPosition = new Vector2(viewport.Width - answersBlockTexture.Width, 485);

            answerBar = new AnswerBar(answersBlockTexture, innerAnswersBlockTexture, grillAnswersBlockTexture,
                answersBlockClosedPosition, answersBlockOpenedPosition, false, false);

            answerBar.innerSpace = new Vector2(21, 28);
            answerBar.answerButtonSpace = new Vector2(10, 10);
            answerBar.BarAnimationStep = 15;
            answerBar.grillAnimationStep = 6;

            // Create life bar
            lifeBar = new LifeBar(2, greenLightTexture, redLightTexture, true);
            lifeBar.Position = new Vector2(392, 5);

            // Create clock bar
            Vector2 bottomBarClosedPosition = new Vector2(0, viewport.Height);
            Vector2 bottomBarOpenedPosition = new Vector2(0, viewport.Height - bottomBarTexture.Height);

            clockBar = new ClockBar(clockFont, bottomBarTexture, maxClockTime, true, bottomBarClosedPosition, bottomBarOpenedPosition, false);
            clockBar.BarAnimationStep = 5;
            clockBar.Color = Color.Red;

            // Create Flashing Text
            flashingText = new FlashingText("", noticeFont);
            flashingText.Color = Color.Red;

            // Create text Notice
            touchNotice = new Text("Touch to Start", noticeFont);

            touchNotice.Position = new Vector2(
                (viewport.Width - touchNotice.Width(this)) / 2,
                (viewport.Height - touchNotice.Height(this)) / 2);
            touchNotice.Color = Color.Red;


            questionParagraph = new Paragraph("", textFont);
            questionParagraph.Capacity = 34;
            InitializeQueryComponents();
        }

        public void InitializeQueryComponents()
        {
            if (queryList.Count != 0)
            {
                currentQuestionNumber++;
                if (queryList.Count > 1)
                    outerListIndex = random.Next(queryList.Count);
                else
                    outerListIndex = 0;

                if (queryList[outerListIndex] != null && queryList[outerListIndex].Count != 0)
                {
                    if (queryList[outerListIndex].Count > 1)
                        innerListIndex = random.Next(queryList[outerListIndex].Count);
                    else
                        innerListIndex = 0;

                    questionParagraph.TextContents = queryList[outerListIndex][innerListIndex].Q;

                    questionBar.QuestionParagraph = questionParagraph;

                    answerButtons.Clear();
                    foreach (string answerStruct in queryList[outerListIndex][innerListIndex].A)
                    {
                        AnswerTextButton button = new AnswerTextButton(
                            answerStruct, textFont, neutralTexture, successTexture, failureTexture );
                        answerButtons.Add(button);
                    }

                    answerBar.AnswerButtons = answerButtons;

                    currentRightAnswer = queryList[outerListIndex][innerListIndex].RA;

                    // Remove query from the inner list
                    queryList[outerListIndex].RemoveAt(innerListIndex);
                }
                else if (queryList[outerListIndex].Count == 0)
                {
                    queryList.RemoveAt(outerListIndex);
                }
            }
        }

        private void InitializeAnimationFields()
        {
            isShowComponents = true;
        }

        #endregion


        #region Handle Input

        public override void HandleInput(InputState input)
        {
            if (IsActive)
            {
                if (input == null)
                    throw new ArgumentNullException("input");

                if (input.IsPauseGame())
                {
                    PauseCurrentGame();
                }
                
                if (ComponentsBarIsOpened && !userTapToStar)
                {
                    if (input.Gestures.Count > 0)
                    {
                        if (input.Gestures[0].GestureType == GestureType.Tap)
                        {
                            userTapToStar = true;
                        }
                    }
                }
                
                if (ComponentsBarIsOpened && ComponentsGrillIsOpened && userTapToStar)
                {
                    if (!gameEnded)
                    {
                        if (!hasAnswered)
                        {
                            foreach (GestureSample gesture in input.Gestures)
                            {
                                if (gesture.GestureType == GestureType.Tap)
                                {
                                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                                    if (!hasAnswered)
                                    {
                                        for (int i = 0; i < answerButtons.Count; i++)
                                        {
                                            if (GetEntryHitBounds(answerButtons[i]).Contains(tapLocation))
                                            {
                                                hasAnswered = true;
                                                if (String.Compare(currentRightAnswer,answerButtons[i].TextContents) == 0)
                                                {
                                                    isUserWon = true;
                                                    answerButtons[i].SetSuccess();
                                                    rightAnswers++;
                                                    if (currentQuestionNumber < maxQuestionNumber)
                                                    {
                                                        AudioManager.StopSounds();
                                                        AudioManager.PlaySound("success");
                                                    }
                                                }
                                                else
                                                {
                                                    answerButtons[i].SetFailure();
                                                    lifeBar.RemoveLife();
                                                    if (!lifeBar.IsLifeEmpty())
                                                    {
                                                        AudioManager.StopSounds();
                                                        AudioManager.PlaySound("failure");
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!userTapToExit)
                        {
                            if (input.Gestures.Count > 0)
                            {
                                if (input.Gestures[0].GestureType == GestureType.Tap)
                                {
                                    AudioManager.StopSounds();

                                    if ((isUserWon || !lifeBar.IsLifeEmpty()  || (clockBar.IsOutOfTime && !lifeBar.IsLifeEmpty()) ) &&  currentQuestionNumber < maxQuestionNumber)
                                    {
                                        nextQuery = true;
                                    }
                                    else
                                    {
                                        isShowComponents = false;
                                    }
                                    userTapToExit = true;
                                }
                            }
                        }
                    }
                }
            }     
        }

        #endregion

        #region Components Bounds

        protected virtual Rectangle GetEntryHitBounds(IComponent component)
        {
            return new Rectangle((int)component.Position.X, (int)component.Position.Y,
                component.Width(this), component.Height(this));
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!IsActive)
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }

            if (moveToHighScore)
            {
                ReplaceAllScreens(
                    new List<GameScreen>()
                    {
                        new BackgroundScreen("highscoreBackground"),
                        new HighScoreScreen()
                    });
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }

            if (insertInHighscore)
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }

            if (CheckIfCurrentGameFinished())
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }

            if (!IsOpenComponentsBar && isShowComponents)
            {
                if(!questionBar.IsOpenBar)
                    questionBar.OpenBar(true);
                if(!answerBar.IsOpenBar)
                    answerBar.OpenBar(true);
                if (questionBar.BarIsOpened && answerBar.BarIsOpened)
                {
                    if (!clockBar.IsOpenBar)
                        clockBar.OpenBar(true);
                }
            }

            if (!ComponentsBarIsClosed && !isShowComponents)
            {
                if (IsOpenComponentsGrill)
                {
                    if (questionBar.IsOpenGrill)
                        questionBar.OpenGrill(false);
                    if (answerBar.IsOpenGrill)
                        answerBar.OpenGrill(false);

                    timeElapsed = timeToWait;
                }

                if (ComponentsGrillIsClosed)
                {
                    if (timeElapsed > TimeSpan.Zero)
                    {
                        timeElapsed -= gameTime.ElapsedGameTime;
                    }

                    if (timeElapsed <= TimeSpan.Zero)
                    {
                        if (clockBar.IsOpenBar)
                            clockBar.OpenBar(false);
                        if (!clockBar.BarIsOpened)
                        {
                            if (questionBar.IsOpenBar)
                                questionBar.OpenBar(false);
                            if (answerBar.IsOpenBar)
                                answerBar.OpenBar(false);
                        }
                    }
                }
            }

            if (ComponentsBarIsOpened && isShowComponents)
            {
                if (!gameEnded && !userTapToExit)
                {
                    if (!userTapToStar)
                    {
                        flashingText.Update(this, gameTime);
                    }
                    else
                    {
                        if (!IsOpenComponentsGrill)
                        {
                            if (!questionBar.IsOpenGrill)
                                questionBar.OpenGrill(true);
                            if (!answerBar.IsOpenGrill)
                                answerBar.OpenGrill(true);

                            timeElapsed = timeToWait;
                        }

                        if (ComponentsGrillIsOpened)
                        {
                            if (clockBar.IsTimeOut)
                            {
                                clockBar.IsTimeOut = false;
                            }
                            else
                            {
                                if (clockBar.ClockTime.Minutes == 0 && clockBar.ClockTime.Seconds < 10 && !isAlarmClock)
                                {
                                    AudioManager.PlaySound("alarmClock", true);
                                    isAlarmClock = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (userTapToExit)
                    {
                        if (ComponentsGrillIsOpened)
                        {
                            if (IsOpenComponentsGrill)
                            {
                                if (questionBar.IsOpenGrill)
                                    questionBar.OpenGrill(false);
                                if (answerBar.IsOpenGrill)
                                    answerBar.OpenGrill(false);

                                timeElapsed = timeToWait;
                            }
                        }

                        if (ComponentsGrillIsClosed)
                        {
                            if (nextQuery)
                            {
                                nextQuery = false;
                                InitializeQueryComponents();
                                clockBar.ResetClock();
                                gameEnded = false;
                                isUserWon = false;
                                hasAnswered = false;
                                isLifeLost = false;
                                isPlaySound = false;
                                isAlarmClock = false;
                            }

                            if (timeElapsed > TimeSpan.Zero)
                            {
                                timeElapsed -= gameTime.ElapsedGameTime;
                            }

                            if (timeElapsed <= TimeSpan.Zero)
                            {
                                userTapToExit = false;
                            }
                        }
                    }
                }
            }

            lifeBar.Update(this, gameTime);
            questionBar.Update(this, gameTime);
            answerBar.Update(this, gameTime);
            clockBar.Update(this, gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            if (!isActive)
            {
                base.Draw(gameTime);
                return;
            }

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);    
            lifeBar.Draw(this, gameTime);
            questionBar.Draw(this, gameTime);
            answerBar.Draw(this, gameTime);
            clockBar.Draw(this, gameTime);

            if (ComponentsBarIsOpened && !userTapToStar)
            {
                flashingText.TextContents = "Touch to Start";
                flashingText.Position = new Vector2(
                (viewport.Width - flashingText.Width(this)) / 2,
                (viewport.Height - flashingText.Height(this)) / 2);
                flashingText.Draw(this, gameTime);
            }

            DrawGameEndIfNecessary();

            spriteBatch.End();
        }

        #endregion

        #region Private Methods Game Logic

        private bool CheckIfCurrentGameFinished()
        {
            if (hasAnswered || isUserWon || clockBar.IsOutOfTime || lifeBar.IsLifeEmpty())
            {
                gameEnded = true;
                clockBar.IsTimeOut = true;
            }

            // if gameEnded, game is over
            if (gameEnded)
            {
                if (!isLifeLost && !isUserWon)
                {
                    if (!lifeBar.IsLifeEmpty() && clockBar.IsOutOfTime)
                    {
                        lifeBar.RemoveLife();

                        isLifeLost = true;
                    }
                }

                if (!isPlaySound)
                {
                    if (lifeBar.IsLifeEmpty())
                    {
                        AudioManager.StopSounds();
                        AudioManager.PlaySound("gameOver");
                        isPlaySound = true;
                    }
                    else if (isUserWon && currentQuestionNumber >= maxQuestionNumber)
                    {
                        AudioManager.StopSounds();
                        AudioManager.PlaySound("gameEndSuccess");
                        isPlaySound = true;
                    }
                    else if (clockBar.IsOutOfTime && !lifeBar.IsLifeEmpty())
                    {
                        AudioManager.StopSounds();
                        AudioManager.PlaySound("failure");
                        isPlaySound = true;
                    }
                    
                }

                if (!isUserWon || lifeBar.IsLifeEmpty() || clockBar.IsOutOfTime || currentQuestionNumber >= maxQuestionNumber)
                {
                    if ((ComponentsBarIsClosed && ComponentsGrillIsClosed) && userTapToExit)
                    {
                        AudioManager.StopSounds();

                        if (!insertInHighscore)
                        {
                            if (CheckIsInHighScore())
                            {
                                if (!Guide.IsVisible)
                                {
                                    Guide.BeginShowKeyboardInput(PlayerIndex.One,
                                        "Player Name", "Insert your name (max 15 characters)!", "Player",
                                        AfterPlayerEnterName, gameEnded);
                                    insertInHighscore = true;
                                }
                            }
                            else
                            {
                                insertInHighscore = true;
                                moveToHighScore = true;
                            }
                        }

                    }
                }
            }

            return false;
        }

        private bool CheckIsInHighScore()
        {
            return HighScoreScreen.IsInHighscores(rightAnswers);
        }

        private void AfterPlayerEnterName(IAsyncResult result)
        {           
            // Get the name entered by the user
            string playerName = Guide.EndShowKeyboardInput(result);

            if (!string.IsNullOrEmpty(playerName))
            {
                // Ensure that it is valid
                if (playerName != null && playerName.Length > 15)
                {
                    playerName = playerName.Substring(0, 15);
                }

                // Puts it in high score
                HighScoreScreen.PutHighScore(playerName, rightAnswers);
            }

            moveToHighScore = true;

            // Moves to the next screen
            //MoveToNextScreen((bool)result.AsyncState);
        }

        private void DrawGameEndIfNecessary()
        {
            if (gameEnded && !userTapToExit)
            {
                string stringToDisplay = string.Empty;

                if (hasAnswered && !lifeBar.IsLifeEmpty())
                {
                    if (isUserWon)
                    {
                        if (currentQuestionNumber == maxQuestionNumber)
                            stringToDisplay = "You Win!";
                        else
                            stringToDisplay = "Right!";
                    }
                    else
                    {
                        if (currentQuestionNumber == maxQuestionNumber)
                            stringToDisplay = "No, it was the last!";
                        else
                            stringToDisplay = "Wrong!";

                    }
                }
                else
                {
                    if (clockBar.IsOutOfTime)
                    {
                        stringToDisplay = "Time Is Up!\n";
                        if (lifeBar.IsLifeEmpty())
                            stringToDisplay = "Time Is Up!\nYou run out\nof Chance!";
                    }
                    else if (lifeBar.IsLifeEmpty())
                    {
                        stringToDisplay = "You run out\nof Chance!";
                    }
                }

                var stringVector = noticeFont.MeasureString(stringToDisplay);

                ScreenManager.SpriteBatch.DrawString(noticeFont, stringToDisplay,
                                new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - stringVector.X / 2,
                                            ScreenManager.GraphicsDevice.Viewport.Height / 2 - stringVector.Y / 2),
                                Color.White);
            }
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        private void PauseCurrentGame()
        {
            IsActive = false;

            // Pause sounds
            AudioManager.PauseResumeSounds(false);

            ScreenManager.AddScreen(new BackgroundScreen("titleBackground"));
            ScreenManager.AddScreen(new PauseScreen(false));
        }

        #endregion

        #region Tombstoning Methods
        /*
        public bool IsSaving()
        {
            if (lifeBar.IsLifeEmpty() || (currentQuestionNumber == maxQuestionNumber && (hasAnswered || clockBar.IsOutOfTime)))
                return false;
            else
                return true;
        }

        public override void Serialize(Stream stream)
        {
            GameData gameData = new GameData();

            if (hasAnswered)
            {
                InitializeQueryComponents();
            }

            gameData.queryList = this.queryList;
            gameData.currentQuery = this.currentQuery;
            gameData.outerListIndex = this.outerListIndex;
            gameData.innerListIndex = this.innerListIndex;
            gameData.rightAnswers = this.rightAnswers;
            gameData.currentQuestionNumber = this.currentQuestionNumber;
            gameData.maxQuestionNumber = this.maxQuestionNumber;
            gameData.clockTime = clockBar.ClockTime;
            gameData.numLife = lifeBar.NumLife;

            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            serializer.Serialize(stream, gameData);
            stream.Close();
            stream.Dispose();
        }

        public override void Deserialize(Stream stream)
        {
            GameData gameData;

            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            gameData = (GameData)serializer.Deserialize(stream);
            stream.Close();
            stream.Dispose();
        }
        */
        #endregion
    }
}
