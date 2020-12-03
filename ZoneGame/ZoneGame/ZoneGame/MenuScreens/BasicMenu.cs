using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ZoneGame
{
    public class BasicMenu : MenuScreen
    {
        #region Fields

        private List<IComponent> verticalComponents = new List<IComponent>();

        private List<IComponent> freeComponents = new List<IComponent>();

        protected int textBlockDistance = 10;
        protected int sideBarAnimationStep = 15;
        protected int glassScreenAnimationStep = 20;

        protected Texture2D blankTexture, buttonMenuTexture, topSideBarTexture, bottomSideBarTexture;

        protected SpriteFont menuFont;

        protected Color textColor = Color.White;

        protected float alphaChannel = 0.2f;

        protected Text title;
        protected Image topSideBar, bottomSideBar;

        bool animateSideBar;
        bool sideBarInTransition;
        bool sideBarHitFinalPosition = false;

        bool glassScreenInTransition;
        bool glassScreenHitFinalPosition = false;


        public bool IsPlayMusic
        {
            get;
            set;
        }

        Vector2 topSideBarClosedPosition;
        Vector2 topSideBarOpenedPosition;

        Vector2 bottomSideBarClosedPosition;
        Vector2 bottomSideBarOpenedPosition;
        Vector2 glassScreenOpenedDimension;

        Rectangle glassScreenDimension;

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
            buttonMenuTexture = contentDynamic.Load<Texture2D>("Textures/Buttons/buttonMenu");
            topSideBarTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/bottomSideBar");
            bottomSideBarTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/topSideBar");
            menuFont = contentDynamic.Load<SpriteFont>("Fonts/MenuFont");

            title = new Text("", menuFont);
            title.Scale = 1.8f;
            title.Color = Color.Yellow;
            Components.Add(title);
            this.animateSideBar = true;

            if (animateSideBar)
            {
                sideBarInTransition = true;
            }
             
            topSideBar = new Image(topSideBarTexture);
            topSideBarClosedPosition = new Vector2(viewport.Width, 0);
            topSideBarOpenedPosition = new Vector2(viewport.Width - topSideBar.Width(), 0);
            topSideBar.Position = topSideBarClosedPosition - new Vector2(30,0);

            bottomSideBar = new Image(bottomSideBarTexture);
            bottomSideBarClosedPosition = new Vector2(viewport.Width, viewport.Height - bottomSideBar.Height());
            bottomSideBarOpenedPosition = new Vector2(viewport.Width - bottomSideBar.Width(), viewport.Height - bottomSideBar.Height());
            bottomSideBar.Position = bottomSideBarClosedPosition - new Vector2(30, 0);

            glassScreenDimension = new Rectangle(0, 0, 0, 0);
            glassScreenOpenedDimension = new Vector2((int)topSideBar.Width(), 
               bottomSideBar.Position.Y + 10 - topSideBar.Position.Y + topSideBar.Height() - 10 );
        }

        #endregion

        #region HandleInput

        public override void HandleInput(InputState input)
        {
            if (glassScreenHitFinalPosition)
            {
                base.HandleInput(input);
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (animateSideBar)
            {
                if (sideBarInTransition)
                    AnimateSideBar();
                if (glassScreenInTransition)
                    AnimateGlassScreen();
                if (glassScreenHitFinalPosition)
                    base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            }
            else
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            }
            
            
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            if (glassScreenInTransition)
            {
                // Draw the GlassScreen
                spriteBatch.Draw(blankTexture, glassScreenDimension, Color.Aqua * 0.6f);
            }
            // Draw the SideBar
            topSideBar.Draw(spriteBatch, gameTime);
            bottomSideBar.Draw(spriteBatch, gameTime);

            spriteBatch.End();

            if(glassScreenHitFinalPosition)
                base.Draw(gameTime);
        }

        #endregion

        #region Methods

        private void AnimateSideBar()
        {
            if (!sideBarHitFinalPosition)
            {
                Vector2 pos = topSideBar.Position;
                 pos.X= MathHelper.Clamp(
                    topSideBar.Position.X - sideBarAnimationStep,
                    topSideBarOpenedPosition.X,
                    topSideBarClosedPosition.X);
                 topSideBar.Position = pos;

                 pos = bottomSideBar.Position;
                 pos.X = MathHelper.Clamp(
                     bottomSideBar.Position.X - sideBarAnimationStep,
                     bottomSideBarOpenedPosition.X,
                     bottomSideBarClosedPosition.X);
                 bottomSideBar.Position = pos;

                 if (topSideBar.Position == topSideBarOpenedPosition && 
                     bottomSideBar.Position == bottomSideBarOpenedPosition)
                 {
                     if (!sideBarHitFinalPosition)
                     {   
                         sideBarHitFinalPosition = true;
                         glassScreenInTransition = true;
                     }
                     else
                     {
                         sideBarHitFinalPosition = false;
                     }
                 }
            }
        }

        private void AnimateGlassScreen()
        {
            if (!glassScreenHitFinalPosition)
            {
                glassScreenDimension = new Rectangle((int)topSideBar.Position.X + 5,
                (int)topSideBar.Position.Y + topSideBar.Height() - 10,
                topSideBar.Width(),
                (int)MathHelper.Clamp(
                    glassScreenDimension.Height + glassScreenAnimationStep,
                    0f,
                    glassScreenOpenedDimension.Y));
                 if (glassScreenDimension.Height == (int)glassScreenOpenedDimension.Y)
                 {
                     if (!glassScreenHitFinalPosition)
                     {   
                         glassScreenHitFinalPosition = true;
                     }
                     else
                     {
                         glassScreenHitFinalPosition = false;
                     }
                 }
            }
        }
        

        protected override void  UpdateComponentsLocation()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

           
            /*
            title.Position = new Vector2((viewport.Width - title.Width(this)) / 2, 50);

            Vector2 position = new Vector2(0f, 125f);
            //Vector2 position = new Vector2(0f, title.Position.Y + title.Font.MeasureString(title.Text).Y + 30 + padding.Top); //Vector2 position = new Vector2(0f, 175f);
            for (int i = 0; i < Components.Count; i++)
            {
                IComponent component = Components[i];
                position.X = viewport.Width / 2 - component.Width(this) / 2;
                component.Position = position;
                position.Y += component.Height(this) + (textBlockDistance * 2) + padding.Bottom;
            }
             * */
            /*
            title.Position = new Vector2((viewport.Width - title.Width(this)) / 2, 50);

            Vector2 verticalPosition = new Vector2(0f, 125f);

            for (int i = 0; i < verticalComponents.Count; i++)
            {
                IComponent component = verticalComponents[i];
                verticalPosition.X = viewport.Width / 2 - component.Width(this) / 2;
                component.Position = verticalPosition;
                verticalPosition.Y += component.Height(this) + (textBlockDistance * 2) + padding.Bottom;
            }
             * */

            title.Position = new Vector2(topSideBar.Position.X + topSideBar.Width() / 2 - title.Width()/2,
                topSideBar.Position.Y + topSideBar.Height()/2  - title.Height()/2);
            
            //Vector2 verticalPosition = new Vector2(0f, 80f);
            Vector2 verticalPosition = new Vector2(0f, topSideBar.Position.Y + topSideBar.Height() + 20);

            for (int i = 0; i < verticalComponents.Count; i++)
            {
                IComponent component = verticalComponents[i];
                verticalPosition.X = topSideBar.Position.X + topSideBar.Width() / 2 - component.Width() / 2;
                component.Position = verticalPosition;
                verticalPosition.Y += component.Height() + (textBlockDistance * 2) + padding.Bottom;
            }
        }

        protected override Rectangle GetEntryHitBounds(IComponent component)
        {
            return new Rectangle((int)component.Position.X - (int)padding.Left, (int)component.Position.Y - (int)padding.Top,
                component.Width() + (int)padding.Right, component.Height() + (int)padding.Bottom);
        }

        protected virtual void AddVerticalTextButton(TextButton textButton)
        {
            textButton.AlphaTexture = alphaChannel;
            textButton.Color = textColor;

            AddVerticalComponent(textButton);
        }

        private void AddVerticalComponent(IComponent component)
        {
            verticalComponents.Add(component);
            Components.Add(component);
        }

        protected void PlayMusic(bool play, string strSong)
        {
            if (play)
            {
                AudioManager.PlayMusic(strSong);
            }
            else
            {
                AudioManager.StopMusic();
            }
        }

        #endregion
    }
}
