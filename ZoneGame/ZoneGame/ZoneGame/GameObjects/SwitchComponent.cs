using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class SwitchComponent : Component
    {
        private bool switchOn;

        Texture2D imageOn, imageOff;

        public bool SwitchOn
        {
            get { return switchOn; }
            set { switchOn = value; }
        }

        public SwitchComponent(Texture2D imageOn, Texture2D imageOff, bool switchOn)
            :base()
        {
            this.switchOn = switchOn;
            this.imageOn = imageOn;
            this.imageOff = imageOff;
        }

        public override void Update(GameTime gameTime) { }        

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {            
            // Draw texture.            
            if (switchOn)
            {                
                spriteBatch.Draw(imageOn, position, sourceRectangle, color * alphaChannel, rotation, origin, scale, effects, 0);                
            }
            else
            {            
                spriteBatch.Draw(imageOff, position, sourceRectangle, color * alphaChannel, rotation, origin, scale, effects, 0);                
            }
        }

        public override int Height()
        {
            if (switchOn)
                return (int)((float)imageOn.Height* scale);                
            else
                return (int)((float)imageOff.Height * scale);         
            
        }

        public override int Width()
        {
            if (switchOn)                
                return (int)((float)imageOn.Width * scale);                
            else                 
                return (int)((float)imageOff.Width * scale);                
        }
    }
}
