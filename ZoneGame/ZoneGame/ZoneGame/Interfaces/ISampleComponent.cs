using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public interface ISampleComponent
    {
        void Update(GameTime gamTime);
        void Draw(SpriteBatch spriteBatch, GameTime gamTime);
    }
}
