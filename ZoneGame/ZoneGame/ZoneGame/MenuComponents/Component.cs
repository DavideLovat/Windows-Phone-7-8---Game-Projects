using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public abstract class Component:IComponent
    {
        #region Fields

        protected int index;
        protected Vector2 position = Vector2.Zero;
        protected Rectangle? sourceRectangle = null;
        protected Color color = Color.White;
        protected Vector2 origin = Vector2.Zero;
        protected float scale = 1f;
        protected float rotation = 0f;
        protected float alphaChannel = 1f;
        protected SpriteEffects effects = SpriteEffects.None;
        protected float layerDepth = 0f;

        #endregion

        #region Properties

        public bool IsActive
        {
            get;
            set;
        }

        public int Index
        {
            get { return index; }
        }

        public Rectangle? SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value;}
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public float AlphaChannel
        {
            get { return alphaChannel; }
            set { alphaChannel = value; }
        }

        public SpriteEffects Effects
        {
            get { return effects; }
            set { effects = value; }
        }

        public float LayerDepth
        {
            get { return layerDepth; }
            set { layerDepth = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        #endregion

        #region Initialization

        public Component()
            : this(0)
        { }

        public Component(int index)
        {
            //this.screen = screen;
            this.index = index;
            /*this.screenManager = screen.ScreenManager;
            graphicDevice = screenManager.GraphicsDevice;
            spriteBatch = screenManager.SpriteBatch;
             */
            IsActive = true;
        }

        #endregion

        #region Interfaces Methods

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        public abstract int Height();

        public abstract int Width();

        #endregion
    }
}
