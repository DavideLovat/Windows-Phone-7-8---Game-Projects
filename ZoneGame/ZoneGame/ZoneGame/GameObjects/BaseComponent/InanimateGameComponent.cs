using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public class InanimateGameComponent : InGameComponent
    {
        #region Fields

        protected Texture2D idleComponentTexture;

        protected Color[] textureData;

        public Color[] TextureData
        {
            get { return textureData; }
            set { textureData = value; }
        }

        #endregion

        #region Properties

        public override Matrix MatrixTransform
        {
            get
            {
               Matrix transform =
                        Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                        // Matrix.CreateScale(block.Scale) *  would go here
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateTranslation(new Vector3(Position, 0.0f));
               return transform;
            }
        }

        public Texture2D IdleComponentTexture
        {
            get { return idleComponentTexture; }

            protected set 
            {
                idleComponentTexture = value;
            }
        }

        public virtual Rectangle BoundsTransform
        {
            get
            {
                // Build the block's transform
                Matrix transform = MatrixTransform;

                // Calculate the bounding rectangle of this block in world space
                Rectangle transformRectangle = CalculateBoundingRectangle(
                         new Rectangle(0, 0, idleComponentTexture.Width, idleComponentTexture.Height),
                         transform);

                return transformRectangle;
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width(), Height());
            }
        }

        public override Rectangle CentralCollisionArea
        {
            get
            {
                Rectangle bounds = Bounds;
                int height = (int)Bounds.Height / 10 * 5;
                int width = (int)Bounds.Width;

                int offsetY = ((int)Bounds.Height - height) / 2;
                int offsetX = ((int)Bounds.Width - width) / 2;

                return new Rectangle(
                    (int)Bounds.X + offsetX,
                    (int)Bounds.Y + offsetY,
                    width,
                    height);
            }
        }

        public override Rectangle BottomCollisionArea
        {
            get
            {
                Rectangle bounds = Bounds;
                int height = (int)Bounds.Height / 10 * 5;
                int width = (int)Bounds.Width;

                int offsetY = ((int)Bounds.Height - height);
                int offsetX = ((int)Bounds.Width - width);

                return new Rectangle(
                    (int)Bounds.X + offsetX,
                    (int)Bounds.Y + offsetY,
                    width,
                    height);
            }
        }

        Vector2 WorldSize
        {
            get { return worldSize; }
            set { worldSize = value; }
        }
        protected Vector2 worldSize;
        #endregion

        #region Initialization

        public InanimateGameComponent(Texture2D idleComponentTexture, Vector2 worldSize)
            :base()
        {
            this.idleComponentTexture = idleComponentTexture;
            this.worldSize = worldSize;
        }

        #endregion

        #region Public Methods

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(idleComponentTexture, Position, SourceRectangle, Color, Rotation, Origin, Scale, Effects, CalcolateLayerDepth());
        }

        public override int Width()
        {
            return idleComponentTexture.Width;
        }

        public override int Height()
        {
            return idleComponentTexture.Height;
        }

        protected Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                        Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        protected virtual float CalcolateLayerDepth()
        {
            return MathHelper.Clamp(((LayerDepthRectangle.Y + LayerDepthRectangle.Height) / worldSize.Y), 0f, 1f);
        }

        #endregion
    }
}

