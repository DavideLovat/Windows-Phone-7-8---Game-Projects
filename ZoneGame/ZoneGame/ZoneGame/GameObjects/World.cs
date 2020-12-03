using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public enum SafeAreaRegion
    {
        Top,
        TopRight,
        TopLeft,
        Left,
        Right,
        Center,
        Bottom,
        BottomRight,
        BottomLeft
    }

    class World
    {
        Vector2 worldSize;        
        Vector2 innerWorldSize;
        Vector2 worldOffset;
        SafeAreaRegion safeRegion;
        Rectangle safeArea;

        public SafeAreaRegion SafeRegion
        {
            get { return safeRegion; }
            set { safeRegion = value; }
        }

        public Rectangle SafeArea
        {
            get { return safeArea; }
            set { safeArea = value; }
        }

        public Vector2 WorldSize
        {
            get { return innerWorldSize + worldOffset * 2; }
        }

        public Vector2 InnerWorldSize
        {
            get { return innerWorldSize; }
            set { innerWorldSize = value; }
        }

        public Vector2 WorldOffset
        {
            get { return worldOffset; }
            set { worldOffset = value; }
        }

        public Rectangle WorldBound
        {
            get 
            {
                return new Rectangle(0, 0, (int)worldSize.X, (int)worldSize.Y);
            }
        }

        public Rectangle InnerWorldBound
        {
            get 
            {
                return new Rectangle(
                    (int)worldOffset.X,
                    (int)worldOffset.Y,
                    (int)innerWorldSize.X,
                    (int)innerWorldSize.Y);
            }
        }

        public World()
        {
            innerWorldSize = Vector2.Zero;
            worldOffset = Vector2.Zero;
            worldSize = Vector2.Zero;
        }

        public World(Vector2 innerSize, Vector2 offset)
        {
            innerWorldSize = innerSize;
            worldOffset = offset;
            worldSize = innerWorldSize + worldOffset * 2;
        }
    }
}
