using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    interface ICollision
    {
        Rectangle Bounds { get; }

        Rectangle CentralCollisionArea { get; }

        Rectangle BottomCollisionArea { get; }
    }
}
