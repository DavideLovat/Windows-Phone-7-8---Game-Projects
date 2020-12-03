using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;
namespace QuizTime
{
    public enum ScreenState
    {
        Active,
        Hidden,
    }

    public abstract class GameScreen
    {
        #region Properties

        //protected string key;
        protected ContentManager contentDynamic;

        protected Dictionary<string, string> LanguageDefinitions = new Dictionary<string, string>();

        public bool isPopup;

        ScreenState screenState = ScreenState.Active;

        public bool isExiting = false;

        bool otherScreenHasFocus;

        ScreenManager screenManager;

        GestureType enabledGestures = GestureType.None;

        bool isSerializable = true;

        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        public bool IsActive
        {
            get { return !otherScreenHasFocus && screenState == ScreenState.Active; }
        }

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        public GestureType EnabledGestures
        {
            get { return enabledGestures; }
            protected set
            {
                enabledGestures = value;
                if (ScreenState == ScreenState.Active)
                {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }

        public bool IsSerializable
        {
            get { return isSerializable; }
            protected set { isSerializable = value; }
        }

        #endregion

        #region Initialization

        public virtual void Initialize() { }

        public virtual void LoadContent() 
        {
            if (contentDynamic == null)
                contentDynamic = new ContentManager(ScreenManager.Game.Services, "Content");
        }

        public virtual void UnloadContent() 
        {
            if(contentDynamic != null)
                contentDynamic.Unload();
        }

        #endregion

        #region Update

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                ScreenManager.RemoveScreen(this);
            }
            else if (coveredByOtherScreen)
            {
                screenState = ScreenState.Hidden;
            }
            else
            {
                screenState = ScreenState.Active;
            }
        }

        #endregion

        #region Draw

        public virtual void Draw(GameTime gameTime) { }

        #endregion

        #region Handle Input

        public virtual void HandleInput(InputState input) { }

        #endregion

        #region Private Methods

        protected void ReplaceForwardScreens(List<GameScreen> forwardScreens)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                if (!(screen is BackgroundScreen))
                    screen.ExitScreen();
            }

            foreach (GameScreen screen in forwardScreens)
            {
                ScreenManager.AddScreen(screen);
            }
        }

        protected void ReplaceAllScreens(List<GameScreen> forwardScreens)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            foreach (GameScreen screen in forwardScreens)
            {
                ScreenManager.AddScreen(screen);
            }
        }

        #endregion

        #region Public Methods

        public virtual void Serialize(Stream stream) { }

        public virtual void Deserialize(Stream stream) { }

        public void ExitScreen()
        {
            isExiting = true;

            ScreenManager.RemoveScreen(this);
        }

        #endregion

        #region Methods

        public virtual void LoadAssets() { } 

        public T Load<T>(string assetName)
        {
            return ScreenManager.Game.Content.Load<T>(assetName);
        }
        #endregion
    }
}
