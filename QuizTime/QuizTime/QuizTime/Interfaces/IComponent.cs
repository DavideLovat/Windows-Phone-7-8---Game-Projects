using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace QuizTime
{
    public interface IComponent
    {
        void Update(GameScreen screen, GameTime gameTime);
        void Draw(GameScreen screen, GameTime gameTime);
        int Height(GameScreen screen);
        int Width(GameScreen screen);
        Vector2 Position { get; set; }
        bool IsActive { get; set; }
    }
}
