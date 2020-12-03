using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class Camera
    {
        //Members
        private GraphicsDevice graphicsDevice;
        private Vector2 m_CameraPosition;
        private RectangleF visibleArea;
        private float m_Zoom;
        private float m_Rotation;
        private Matrix m_Transform;       

        #region Set/Get

        /// <summary>
        /// Camera Zoom amount
        /// </summary>
        public float Zoom
        {
            get { return m_Zoom; }
            set { m_Zoom = value; if (m_Zoom < 0.1f) m_Zoom = 0.1f; } // Negative zoom will flip image
        }

        /// <summary>
        /// Camera Rotation amount
        /// </summary>
        public float Rotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
        }

        /// <summary>
        /// Moves the camera with the input amount
        /// </summary>
        /// <param name="amount"></param>
        public void Move(Vector2 amount)
        {
            m_CameraPosition += amount;
        }

        /// <summary>
        /// Camera Postion
        /// </summary>
        public Vector2 Position
        {
            get { return m_CameraPosition; }
            set 
            { 
                m_CameraPosition = value;
                visibleArea.X = m_CameraPosition.X - visibleArea.Width / 2;
                visibleArea.Y = m_CameraPosition.Y - visibleArea.Height / 2;
            }
        }

        public Vector2 ScreenPosition
        {
            get
            {
                return new Vector2(
                    graphicsDevice.Viewport.Width / 2,
                    graphicsDevice.Viewport.Height / 2);
            }
        }

        #endregion

        #region Culling

        public RectangleF VisibleArea
        {
            get { return visibleArea; }
        }

        public Vector2 ViewingPosition
        {
            get 
            {
                return new Vector2(visibleArea.X, visibleArea.Y); 
            }

            set
            {
                Vector2 newVec = value;
                visibleArea.X = newVec.X;
                visibleArea.Y = newVec.Y;
                m_CameraPosition = newVec + new Vector2(
                    graphicsDevice.Viewport.Width / 2,
                    graphicsDevice.Viewport.Height / 2);
            }
        }

        public float ViewingPositionX
        {
            get { return visibleArea.X; }
            set 
            {
                float valueX = value;
                visibleArea.X = valueX;
                m_CameraPosition.X = valueX + graphicsDevice.Viewport.Width / 2;
            }
        }

        public float ViewingPositionY
        {
            get { return visibleArea.Y; }
            set 
            {
                float valueY = value;
                visibleArea.Y = valueY;
                m_CameraPosition.Y = valueY + graphicsDevice.Viewport.Height / 2;
            }
        }

        public float ViewingWidth
        {
            get { return visibleArea.Width; }
            set { visibleArea.Width = value; }
        }

        public float ViewingHeight
        {
            get { return visibleArea.Height; }
            set { visibleArea.Height = value; }
        }

        #endregion Culling

        #region Constructors

        public Camera(GraphicsDevice graphics)
        {
            graphicsDevice = graphics;
            m_Zoom = 1;
            m_Rotation = 0;
            visibleArea = new RectangleF(
                0,
                0,
                graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height);
            m_CameraPosition = ScreenPosition;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Updates the cameras transform
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        public Matrix Transform()
        {            
            float ViewportWidth = graphicsDevice.Viewport.Width;
            float ViewportHeight = graphicsDevice.Viewport.Height; 
 
            m_Transform =
              Matrix.CreateTranslation(new Vector3(-m_CameraPosition.X, -m_CameraPosition.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) *
                                         Matrix.CreateTranslation(new Vector3(ViewportWidth * 0.5f, ViewportHeight * 0.5f, 0));
            return m_Transform;
        }
        #endregion Methods
    }
}
