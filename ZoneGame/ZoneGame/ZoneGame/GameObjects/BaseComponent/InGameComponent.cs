using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public abstract class InGameComponent : ISampleComponent, ICollision
    {
        #region Fields

        protected Vector2 position = Vector2.Zero;
        protected Vector2 origin = Vector2.Zero;

        protected Color color = Color.White;
        
        protected float scale = 1f;
        protected float rotation = 0f;
        protected float alphaChannel = 1f;
        protected float layerDepth = 0f;

        protected Rectangle destinationRectangle = Rectangle.Empty;

        protected Rectangle? sourceRectangle = null;

        protected SpriteEffects effects = SpriteEffects.None;

        #endregion

        #region Properties

        public virtual Matrix MatrixTransform
        {
            get { return Matrix.CreateTranslation(new Vector3(Position, 0.0f)); }
        }

        public virtual Rectangle LayerDepthRectangle
        {
            get { return Rectangle.Empty; }
        }

        public virtual Rectangle Bounds 
        {
            get { return Rectangle.Empty; }
        }

        public virtual Rectangle CentralCollisionArea
        {
            get { return Rectangle.Empty; }
        }

        public virtual Rectangle BottomCollisionArea
        {
            get { return Rectangle.Empty; }
        }

        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
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
            set { scale = value; }
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

        public float LayerDepth
        {
            get { return layerDepth; }
            set { layerDepth = value; }
        }

        public Rectangle DestinationRectangle
        {
            get { return destinationRectangle; }
            set { destinationRectangle = value; }
        }

        public Rectangle? SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }

        public SpriteEffects Effects
        {
            get { return effects; }
            set { effects = value; }
        }



        #endregion

        #region Initialization

        public InGameComponent() { }

        #endregion

        #region Public Methods

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }

        public virtual int Width() { return 0; }

        public virtual int Height() { return 0; }

        #endregion
    }
}
