using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class LifeBar : Component
    {
        #region Fields

        SwitchComponent[] switchComponents;
        Texture2D imageOn, imageOff;

        int maxLife;

        int numLife;

        public int NumLife
        {
            get { return numLife; }
        }

        bool switchOn;

        #endregion

        #region Initialization

        public LifeBar(int maxLife, Texture2D imageOn, Texture2D imageOff, bool switchOn)
        {
            this.maxLife = maxLife;
            this.imageOn = imageOn;
            this.imageOff = imageOff;
            this.switchOn = switchOn;
            numLife = maxLife;
            switchComponents = new SwitchComponent[maxLife];
            for (int i = 0; i < switchComponents.Length; i++)
            {
                switchComponents[i] =
                    new SwitchComponent(imageOn, imageOff, switchOn);
            }
        }

        #endregion

        #region Update

        public override void Update(GameScreen screen, GameTime gameTime)
        {
            base.Update(screen, gameTime);
        }

        #endregion

        #region Draw

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            base.Draw(screen, gameTime);

            for (int i = 0; i < switchComponents.Length; i++)
            {
                switchComponents[i].Position =
                    Position - new Vector2((switchComponents[i].Width(screen) + 20) * i, 0);
            }

            for (int i = 0; i < switchComponents.Length; i++)
            {
                switchComponents[i].Draw(screen, gameTime);
            }
        }

        #endregion

        #region Public Methods

        public override int Height(GameScreen screen)
        {
            int height = 0;
            if (switchComponents.Length > 0)
                height = switchComponents[0].Height(screen);
            return (int)height;
        }

        public override int Width(GameScreen screen)
        {
            int width = 0;
            for (int i = 0; i < switchComponents.Length; i++)
            {
                width += (switchComponents[i].Width(screen) + 20) * i;
            }

            return width;
        }

        public bool IsLifeEmpty()
        {
            return numLife <= 0;
        }

        public void RemoveLife()
        {
            if (numLife > 0)
            {
                int index = (maxLife - numLife);
                switchComponents[index].SwitchOn = !switchOn;
            }

            MathHelper.Clamp(numLife--, 0, maxLife);
            
        }   

        #endregion

        #region Protected Methods



        #endregion
    }
}
