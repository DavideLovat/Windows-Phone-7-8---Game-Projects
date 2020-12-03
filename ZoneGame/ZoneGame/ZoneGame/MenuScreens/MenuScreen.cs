using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;

namespace ZoneGame
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

        protected IList<IComponent> Components
        {
            get { return components; }
        }
        /*
        protected IComponent Title
        {
            get { return title; }
        }
        
        protected SpriteFont MenuFont
        {
            get { return menuFont; }
            set { menuFont = value; }
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
            /*
            if (contentDynamic == null)
                contentDynamic = new ContentManager(ScreenManager.Game.Services, "Content");
            */
            /*
            menuFont = contentDynamic.Load<SpriteFont>("Fonts/MenuFont");

            title = new Text("", menuFont);
            */
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
                        ISelectable component = components[i] as ISelectable;
                        if (component != null)
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

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //title.Update(this, gameTime);

            for (int i = 0; i < components.Count; i++)
                components[i].Update(gameTime);
            
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
                IComponent component = components[i];
                component.Draw(spriteBatch, gameTime);
            }

            spriteBatch.End();
        }

        #endregion

        #region Methods

        protected virtual void UpdateComponentsLocation() { }

        protected virtual Rectangle GetEntryHitBounds(IComponent component)
        {
            return new Rectangle((int)component.Position.X - (int)padding.Left, (int)component.Position.Y - (int)padding.Top,
                component.Width() + (int)padding.Right, component.Height() + (int)padding.Bottom);
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

        #endregion
    }
}
