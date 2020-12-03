using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
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

        public SwitchComponent(int index, Texture2D imageOn, Texture2D imageOff ,bool switchOn)
            :base(index)
        {
            this.switchOn = switchOn;
            this.imageOn = imageOn;
            this.imageOff = imageOff;
        }

        public override void Update(GameScreen screen, GameTime gameTime)
        {
            base.Update(screen, gameTime);
        }

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            base.Draw(screen, gameTime);
            // Draw texture.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            if (switchOn)
            {
                if (imageOn != null)
                    spriteBatch.Draw(imageOn, position, sourceRectangle, color * alphaChannel, rotation, origin, scale, effects, 0);
                else if(imageOff != null)
                    spriteBatch.Draw(imageOff, position, sourceRectangle, color * alphaChannel, rotation, origin, scale, effects, 0);
            }
            else
            {
                if (imageOff != null)
                    spriteBatch.Draw(imageOff, position, sourceRectangle, color * alphaChannel, rotation, origin, scale, effects, 0);
                else if (imageOn != null)
                    spriteBatch.Draw(imageOn, position, sourceRectangle, color * alphaChannel, rotation, origin, scale, effects, 0);
            }
        }

        public override int Height(GameScreen screen)
        {
            if (switchOn)
                if (imageOn != null)
                    return (int)((float)imageOn.Height* scale);
                else if (imageOff != null)
                    return (int)((float)imageOff.Height * scale);
                else
                    return 0;
            else
                if (imageOff != null)
                    return (int)((float)imageOff.Height * scale);
                else if (imageOn != null)
                    return (int)((float)imageOn.Height * scale);
                else
                    return 0;
            
        }

        public override int Width(GameScreen screen)
        {
            if (switchOn)
                if (imageOn != null)
                    return (int)((float)imageOn.Width * scale);
                else if (imageOff != null)
                    return (int)((float)imageOff.Width * scale);
                else
                    return 0;
            else 
                if (imageOff != null)
                    return (int)((float)imageOff.Width * scale);
                else if (imageOn != null)
                    return (int)((float)imageOn.Width * scale);
                else
                    return 0;
        }
    }
}
