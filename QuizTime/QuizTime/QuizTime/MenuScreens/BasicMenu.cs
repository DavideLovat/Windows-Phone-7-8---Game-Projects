using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;

namespace QuizTime
{
    public class BasicMenu : GameScreen
    {
        #region Fields
    
        protected int buttonDistance = 32;
        protected int sideBarAnimationStep = 15;

        protected Texture2D blankTexture, buttonMenuTexture, titleMenuTexture;

        protected SpriteFont menuFont;

        protected Color textColor = Color.White;

        protected float alphaChannel = 0.8f;

        protected Image title;

        int posY = 442;

        bool animateSideBar;
        bool sideBarIsActive = false;
        bool sideBarInTransition;
        bool sideBarHitFinalPosition = false;

        List<IComponent> sideBarsList = new List<IComponent>();

        List<Vector2> SideBarsClosedPosition = new List<Vector2>();
        List<Vector2> SideBarsOpenedPosition = new List<Vector2>();

        #endregion

        #region Properties

        protected IComponent Title
        {
            get { return title; }
        }

        protected SpriteFont MenuFont
        {
            get { return menuFont; }
            set { menuFont = value; }
        }

        #endregion

        #region Initialization

        public BasicMenu()
            : base()
        {
            EnabledGestures = GestureType.Tap;
        }
        /*
        public BasicMenu(bool animateSideBar)
            : base()
        {
            this.animateSideBar = animateSideBar;
        }
        */
        public override void LoadContent()
        {
            base.LoadContent();

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            blankTexture = contentDynamic.Load<Texture2D>("Textures/blank");
            titleMenuTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/menuTitle");
            menuFont = contentDynamic.Load<SpriteFont>("Fonts/MenuFont");

            title = new Image(titleMenuTexture);
            title.Position = new Vector2((viewport.Width - titleMenuTexture.Width) / 2, 174);
            this.animateSideBar = true;

            if (animateSideBar)
            {
                sideBarInTransition = true;
            }

            //SideBar.Position = topSideBarClosedPosition - new Vector2(30,0);
        }

        #endregion

        #region HandleInput

        public override void HandleInput(InputState input)
        {
            if (sideBarHitFinalPosition)
            {
                if (input == null)
                    throw new ArgumentNullException("input");

                if (input.IsNewButtonPress(Buttons.Back))
                {
                    OnCancel();
                }

                foreach (GestureSample gesture in input.Gestures)
                {
                    if (gesture.GestureType == GestureType.Tap)
                    {
                        Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                        for (int i = 0; i < sideBarsList.Count; i++)
                        {
                            if (sideBarsList[i].IsActive)
                            {
                                ISelectable component = sideBarsList[i] as ISelectable;
                                if (component != null)
                                {
                                    if (component.IsSelectable)
                                    {
                                        if (GetEntryHitBounds(component as IComponent).Contains(tapLocation))
                                        {
                                            component.OnSelected();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (animateSideBar)
            {
                if (sideBarInTransition)
                {
                    AnimateSideBar();
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            title.Draw(this, gameTime);

            if (animateSideBar)
            {
                // Draw the SideBar
                for (int i = 0; i < sideBarsList.Count; i++)
                {
                    sideBarsList[i].Draw(this, gameTime);  
                }
            }

            spriteBatch.End();
        }

        #endregion

        #region Methods

        private void AnimateSideBar()
        {
            if (!sideBarHitFinalPosition)
            {
                bool ready = true;

                for (int i = 0; i < sideBarsList.Count; i++)
                {
                    Vector2 pos = sideBarsList[i].Position;
                    pos.X = MathHelper.Clamp(
                        pos.X - sideBarAnimationStep,
                        SideBarsOpenedPosition[i].X,
                        SideBarsClosedPosition[i].X);
                    sideBarsList[i].Position = pos;

                    if (ready)
                    {
                        ready = sideBarsList[i].Position == SideBarsOpenedPosition[i];
                    }
                }

                if (ready)
                {
                    if (!sideBarHitFinalPosition)
                     {   
                         sideBarHitFinalPosition = true;
                     }
                     else
                     {
                         sideBarHitFinalPosition = false;
                     }
                }
            }
        }

        protected void AddSideBarComponent(IComponent component)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            sideBarsList.Add(component);
            SideBarsClosedPosition.Add(new Vector2(viewport.Width, posY));
            SideBarsOpenedPosition.Add(new Vector2(viewport.Width - component.Width(this), posY));
            posY += buttonDistance + component.Height(this);
            component.Position = SideBarsClosedPosition.Last<Vector2>();
        }

        protected virtual Rectangle GetEntryHitBounds(IComponent component)
        {
            return new Rectangle((int)component.Position.X, (int)component.Position.Y,
                component.Width(this), component.Height(this));
        }

        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        #endregion
    }
}
