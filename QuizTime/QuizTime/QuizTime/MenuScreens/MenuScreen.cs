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

    public abstract class MenuScreen:GameScreen
    {
        #region Fields

        //protected SpriteFont menuFont;

        List<IComponent> components = new List<IComponent>();

        protected Padding padding = Padding.Zero;

        //protected Text title;

        #endregion

        #region Properties
        /*
        protected IList<IComponent> Components
        {
            get { return components; }
        }
         */
        #endregion

        #region Initialization

        public MenuScreen()
            :base()
        {
            EnabledGestures = GestureType.Tap;
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        #endregion

        #region HandleInput

        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if(input.IsNewButtonPress(Buttons.Back))
            {
                OnCancel();
            }

            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    for (int i = 0; i < components.Count; i++)
                    {
                        if (components[i].IsActive)
                        {
                            ISelectable component = components[i] as ISelectable;
                            if (component != null)
                            {
                                if (component.IsSelectable)
                                {
                                    if (GetEntryHitBounds(component as IComponent).Contains(tapLocation))
                                    {
                                        OnSelectEntry(i);
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

            //title.Update(this, gameTime);

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].IsActive)
                {
                    components[i].Update(this, gameTime);
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            UpdateComponentsLocation();

            spriteBatch.Begin();

            //title.Draw(this, gameTime);

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].IsActive)
                {
                    components[i].Draw(this, gameTime);
                }
            }

            spriteBatch.End();
        }

        #endregion

        #region Methods

        protected virtual void UpdateComponentsLocation() { }

        protected virtual Rectangle GetEntryHitBounds(IComponent component)
        {
            return new Rectangle((int)component.Position.X - (int)padding.Left, (int)component.Position.Y - (int)padding.Top,
                component.Width(this) + (int)padding.Right, component.Height(this) + (int)padding.Bottom);
        }

        protected virtual void OnSelectEntry(int entryIndex)
        {
            ISelectable selectable = components[entryIndex] as ISelectable;
            selectable.OnSelected();
        }

        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        protected virtual void AddComponent(IComponent component)
        {
            components.Add(component);
        }

        #endregion
    }
}
