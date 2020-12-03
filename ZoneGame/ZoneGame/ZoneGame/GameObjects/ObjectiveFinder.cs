using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class ObjectiveFinder : InGameComponent
    {
        Texture2D baseTexture, arrowTexture;        
        Vector2 center;
       
        public Vector2 Center
        {
            get { return center; }
        }

        public Vector2 CenterPosition
        {
            get { return position + center; }
        }

        List<Arrow> arrows = new List<Arrow>();
        public List<Arrow> Arrows
        {
            get { return arrows; }
            set { arrows = value; }
        }
        Vector2 arrowOrigin;
        float distanceRadius;

        public ObjectiveFinder(Texture2D baseTexture, Texture2D arrowTexture)
        {
            this.baseTexture = baseTexture;
            this.arrowTexture = arrowTexture;
            center = new Vector2(baseTexture.Width / 2, baseTexture.Height / 2) + Position;
            arrowOrigin = new Vector2(0, arrowTexture.Height / 2);
            distanceRadius = (float)Math.Sqrt(arrowTexture.Width * arrowTexture.Width + arrowTexture.Height * arrowTexture.Height) * .75f;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Arrow arrow in arrows)
            {
                arrow.Position = Position + Center;
                arrow.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Arrow arrow in arrows)
            {
                arrow.Draw(spriteBatch, gameTime);
            }

            spriteBatch.Draw(baseTexture, Position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        protected bool Distance(Vector2 point1, Vector2 point2)
        {
            return Vector2.Distance(point1, point2) > distanceRadius;
        }

        public void AddObjectives(InGameComponent obj)
        {
            Arrow arrow = new Arrow(arrowTexture, obj);            
            arrow.Origin = arrowOrigin;
            arrow.DistanceRadius = 100;
            arrows.Add(arrow);
        }

        public void RemoveObjectives(InGameComponent obj)
        {
            List<Arrow> arrowToRemove = new List<Arrow>();

            foreach (Arrow arrow in arrows)
            {
                if (arrow.Objective == obj)
                {
                    arrowToRemove.Add(arrow);
                }
            }

            foreach (Arrow arrow in arrowToRemove)
                arrows.Remove(arrow);
        }

        public override int Width()
        {
            return baseTexture.Height;
        }

        public override int Height()
        {
            return baseTexture.Width;
        }
    }
}
