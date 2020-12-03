using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ZoneGame
{
    class Arrow : InGameComponent
    {
        bool hide = false;

        Texture2D arrowTexture;
        float distanceRadius = 10;
        public float DistanceRadius
        {
            get { return distanceRadius; }
            set 
            {
                if (value == 0)
                    distanceRadius = 10;
                else
                    distanceRadius = value;
            }
        }
        InGameComponent objective;
        public InGameComponent Objective
        {
            get { return objective; }
        }

        public Arrow(Texture2D text, InGameComponent obj)
        {
            arrowTexture = text;
            objective = obj;
        }

        public override void Update(GameTime gameTime)
        {
            CalcolateArrowsRotation();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(!hide)
                spriteBatch.Draw(arrowTexture, Position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        protected void CalcolateArrowsRotation()
        {            
            Vector2 dir = Vector2.Zero;
            Vector2 centerPos = 
                objective.Position + 
                new Vector2(objective.Width() / 2, objective.Height() / 2);

            dir =  centerPos - Position;
            if (dir.Length() > distanceRadius)
            {
                hide = false;
                rotation = (float)Math.Atan2(dir.Y, dir.X);
            }
            else
            {
                hide = true;
            }
        }
    }
}
