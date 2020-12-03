using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ZoneGame
{
    public interface IComponent : ISampleComponent
    {        
        int Height();
        int Width();
        Vector2 Position { get; set; }
    }
}
