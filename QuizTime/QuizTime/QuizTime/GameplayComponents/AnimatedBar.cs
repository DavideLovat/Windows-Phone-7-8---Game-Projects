using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    abstract class  AnimatedBar : Component
    {
        #region Fields

        protected new Vector2 position;

        protected new Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        Vector2 closedPosition, openedPosition;

        bool barIsOpened;

        public bool BarIsOpened
        {
            get { return barIsOpened; }

            set
            {
                barIsOpened = value;

                if (barIsOpened)
                {
                    Position = openedPosition;
                }
                else
                {
                    Position = closedPosition;
                }
            }
        }

        bool isAnimateBar;
        bool isOpenBar;

        public bool IsOpenBar
        {
            get { return isOpenBar; }
            private set { isOpenBar = value; }
        }

        bool barHitFinalPosition;

        int barAnimationStep = 5;

        public int BarAnimationStep
        {
            get { return barAnimationStep; }
            set { barAnimationStep = value; }
        }

        #endregion

        #region Initialization

        public AnimatedBar(Vector2 closedPosition, Vector2 openedPosition, bool barIsOpened)
            :base()
        {
            this.closedPosition = closedPosition;
            this.openedPosition = openedPosition;

            BarIsOpened = barIsOpened;
        }

        public AnimatedBar(int index, Vector2 closedPosition, Vector2 openedPosition, bool barIsOpened)
            :base(index)
        {
            this.closedPosition = closedPosition;
            this.openedPosition = openedPosition;

            BarIsOpened = barIsOpened;
        }

        #endregion

        #region Update

        public override void Update(GameScreen screen, GameTime gameTime)
        {
            base.Update(screen, gameTime);

            // Animate bar
            AnimateBar(screen);
        }

        #endregion

        #region Public Methods
        /*
        public override int Height(GameScreen screen)
        {
            throw new NotImplementedException();
        }

        public override int Width(GameScreen screen)
        {
            throw new NotImplementedException();
        }
        */
        #endregion

        #region Animation Methods

        public void OpenBar(bool open)
        {
            isAnimateBar = true;
            isOpenBar = open;
            barHitFinalPosition = false;
        }

        private void AnimateBar(GameScreen screen)
        {
            if (isAnimateBar)
            {
                if (!barHitFinalPosition)
                {
                    AudioManager.PlaySound("sideBarMovement",true);

                    Vector2 path = Vector2.Zero;
                    Vector2 startPoint = Vector2.Zero;
                    Vector2 finishPoint = Vector2.Zero;

                    if (isOpenBar)
                    {
                        path = openedPosition - closedPosition;
                        startPoint = closedPosition;
                        finishPoint = openedPosition;
                    }
                    else
                    {
                        path = closedPosition - openedPosition;
                        startPoint = openedPosition;
                        finishPoint = closedPosition;
                    }

                    Vector2 dir = Vector2.Normalize(path);

                    Position += barAnimationStep * dir;

                    if ((Position - startPoint).LengthSquared() > path.LengthSquared())
                    {
                        Position = finishPoint;
                    }

                    if (isOpenBar)
                    {
                        if (Position == openedPosition)
                        {
                            if (!barHitFinalPosition)
                            {
                                barHitFinalPosition = true;
                                isAnimateBar = false;
                                BarIsOpened = true;
                                AudioManager.StopSound("sideBarMovement");
                                AudioManager.PlaySound("sideBarHitPosition");
                            }
                            else
                            {
                                barHitFinalPosition = false;
                            }
                        }
                    }
                    else
                    {
                        if (Position == closedPosition)
                        {
                            if (!barHitFinalPosition)
                            {
                                barHitFinalPosition = true;
                                isAnimateBar = false;
                                BarIsOpened = false;
                                AudioManager.StopSound("sideBarMovement");
                                AudioManager.PlaySound("sideBarHitPosition");
                            }
                            else
                            {
                                barHitFinalPosition = false;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
