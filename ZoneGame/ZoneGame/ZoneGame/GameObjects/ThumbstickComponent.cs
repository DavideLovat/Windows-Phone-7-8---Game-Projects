using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class ThumbstickComponent : ISampleComponent
    {
        private Texture2D controlstickBoundary;
        private Texture2D controlstick;

        private Vector2 controlStickBoundaryPosition;

        public Vector2 ControlStickBoundaryPosition
        {
            get { return controlStickBoundaryPosition; }
            set 
            { 
                controlStickBoundaryPosition = value; 
                controlStickStartedPosition = new Vector2(
                    controlStickBoundaryPosition.X + 21,
                    controlStickBoundaryPosition.Y + 22);
                controlStickPosition = controlStickStartedPosition;
            }

        }

        private Vector2 controlStickStartedPosition;

        public Vector2 ControlStickStartedPosition
        {
            get { return controlStickStartedPosition; }
        }

        private Vector2 controlStickPosition;

        public Vector2 ControlStickPosition
        {
            get { return controlStickPosition; }
            set { controlStickPosition = value; }
        }

        public Rectangle ControlStickBoundaryDimension
        {
            get
            {
                return new Rectangle(
                    (int)controlStickBoundaryPosition.X,
                    (int)controlStickBoundaryPosition.Y,
                    controlstickBoundary.Width,
                    controlstickBoundary.Height);
            }
        }

        public Rectangle ControlStickDimension
        {
            get
            {
                return new Rectangle(
                    (int)controlStickPosition.X,
                    (int)controlStickPosition.Y,
                    controlstick.Width,
                    controlstick.Height
                    );
            }
        }

        public ThumbstickComponent(Texture2D controlstickBoundary, Texture2D controlstick)
        {
            this.controlstickBoundary = controlstickBoundary;
            this.controlstick = controlstick;
        }

        public void Update(GameTime gamTime)
        { }

        public void Draw(SpriteBatch spriteBatch, GameTime gamTime)
        {
            spriteBatch.Draw(controlstickBoundary, controlStickBoundaryPosition, Color.White);
            spriteBatch.Draw(controlstick, controlStickPosition, Color.White);
        }

        public void ResetControlStickPosition()
        {
            controlStickPosition = controlStickStartedPosition;
        }
    }
}
