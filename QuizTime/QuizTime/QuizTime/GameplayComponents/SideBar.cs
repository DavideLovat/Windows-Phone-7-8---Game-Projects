using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class SideBar : AnimatedBar
    {
        #region Fields

        Texture2D outerBlockTexture, innerBlockTexture, grillBlockTexture;
        
        protected Image outerBlock, innerBlock, grillBlock;

        public Vector2 OuterSpace
        {
            get;
            set;
        }  
        public Vector2 innerSpace 
        {
            get;
            set;
        }
        public Vector2 grillSpace 
        {
            get;
            set;
        }

        Rectangle grillBlockClosedDimension, grillBlockOpenedDimension;

        bool grillIsOpened;

        public bool GrillIsOpened
        {
            get { return grillIsOpened; }
            set 
            {
                grillIsOpened = value;
                
                if (grillIsOpened)
                {
                    grillBlock.SourceRectangle = grillBlockOpenedDimension;
                }
                else
                {
                    grillBlock.SourceRectangle = grillBlockClosedDimension;
                }
            }
        }

        #endregion

        #region Animation Fields

        bool isAnimateGrill;
        bool isOpenGrill;

        public bool IsOpenGrill
        {
            get { return isOpenGrill; }
            private set { isOpenGrill = value; }
        }

        bool grillHitFinalPosition;

        public int grillAnimationStep = 5;

        int GrillAnimationStep
        {
            get { return grillAnimationStep; }
            set { grillAnimationStep = value; }
        }

        #endregion

        #region Initialization

        public SideBar(Texture2D outerBlockTexture, Texture2D innerBlockTexture, Texture2D grillBlockTexture,
            Vector2 closedPosition, Vector2 openedPosition, bool sideBarIsOpened, bool grillIsOpened)
            :base(closedPosition, openedPosition, sideBarIsOpened)
        {
            InitializeConstructor(outerBlockTexture, innerBlockTexture, grillBlockTexture, grillIsOpened);
        }

        public SideBar(int index, Texture2D outerBlockTexture, Texture2D innerBlockTexture, Texture2D grillBlockTexture,
            Vector2 closedPosition, Vector2 openedPosition, bool sideBarIsOpened, bool grillIsOpened)
            : base(index, closedPosition, openedPosition, sideBarIsOpened)
        {
            InitializeConstructor(outerBlockTexture, innerBlockTexture, grillBlockTexture, grillIsOpened);
        }

        private void InitializeConstructor(Texture2D outerBlockTexture, Texture2D innerBlockTexture, Texture2D grillBlockTexture, bool grillIsOpened)
        {
            this.outerBlockTexture = outerBlockTexture;
            this.innerBlockTexture = innerBlockTexture;
            this.grillBlockTexture = grillBlockTexture;

            grillBlockClosedDimension =
                new Rectangle(
                0,
                0,
                grillBlockTexture.Width,
                grillBlockTexture.Height);
            grillBlockOpenedDimension =
                new Rectangle(
                    0,
                    grillBlockTexture.Height,
                    grillBlockTexture.Width,
                    0);

            outerBlock = new Image(outerBlockTexture);
            innerBlock = new Image(innerBlockTexture);
            grillBlock = new Image(grillBlockTexture);

            GrillIsOpened = grillIsOpened;
        }

        #endregion

        #region Update

        public override void Update(GameScreen screen, GameTime gameTime)
        {
            base.Update(screen, gameTime);

            // Animate grill
            AnimateGrill(screen);
        }

        #endregion

        #region Draw

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            base.Draw(screen, gameTime);

            // Update depending position
            UpdateDependingPositionComponent(screen, gameTime);

            // Draw Block
            outerBlock.Draw(screen, gameTime);
            // Draw inner Block
            innerBlock.Draw(screen, gameTime);

            // Draw Content
            DrawContent(screen, gameTime);

            // Draw grill
            grillBlock.Draw(screen, gameTime);
        }

        #endregion


        #region Public Methods

        public override int Height(GameScreen screen)
        {
            return outerBlock.Height(screen);
        }

        public override int Width(GameScreen screen)
        {
            return outerBlock.Width(screen);
        }

        #endregion

        #region Protected Methods

        protected virtual void UpdateDependingPositionComponent(GameScreen screen, GameTime gameTime)
        {
            outerBlock.Position = Position + OuterSpace;
            innerBlock.Position = outerBlock.Position + innerSpace;
            grillBlock.Position = innerBlock.Position + grillSpace;
        }

        protected virtual void DrawContent(GameScreen screen, GameTime gameTime) { }

        #endregion

        #region Animation Methods

        public void OpenGrill(bool open)
        {
            isAnimateGrill = true;
            isOpenGrill = open;
            grillHitFinalPosition = false;
        }

        private void AnimateGrill(GameScreen screen)
        {
            if (isAnimateGrill)
            {
                if (!grillHitFinalPosition)
                {
                    // Playsound movement 
                    AudioManager.PlaySound("grillMovement", true);

                    Rectangle rect = Rectangle.Empty;
                    int y;
                    int step;

                    step = isOpenGrill ? grillAnimationStep : -grillAnimationStep;
                    rect = (Rectangle)grillBlock.SourceRectangle;
                    y = (int)MathHelper.Clamp(rect.Y + step,
                            0, grillBlock.Height(screen));

                     rect =
                        new Rectangle(
                            rect.X,
                            y,
                            rect.Width,
                            grillBlock.Height(screen) - y);                    

                    grillBlock.SourceRectangle = rect;

                    if (isOpenGrill)
                    {
                        if (rect.Height == grillBlockOpenedDimension.Height)
                        {
                            if (!grillHitFinalPosition)
                            {
                                grillHitFinalPosition = true;
                                isAnimateGrill = false;
                                GrillIsOpened = true;
                                AudioManager.StopSound("grillMovement");
                                AudioManager.PlaySound("grillHitPosition");
                            }
                            else
                            {
                                grillHitFinalPosition = false;
                            }
                        }
                    }
                    else
                    {
                        if (rect.Height == grillBlockClosedDimension.Height)
                        {
                            if (!grillHitFinalPosition)
                            {
                                grillHitFinalPosition = true;
                                isAnimateGrill = false;
                                GrillIsOpened = false;
                                AudioManager.StopSound("grillMovement");
                                AudioManager.PlaySound("grillHitPosition");
                            }
                            else
                            {
                                grillHitFinalPosition = false;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
