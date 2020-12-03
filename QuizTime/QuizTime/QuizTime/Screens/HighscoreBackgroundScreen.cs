using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class HighscoreBackgroundScreen : GameScreen
    {
        #region Fields

        Texture2D leftDoorTexture, rightDoorTexture;

        Image leftDoor, rightDoor;

        int doorsAnimationStep = 3;

        bool animateDoors;
        bool doorsInTransition;
        bool doorsHitFinalPosition = false;

        Vector2 leftDoorOpenedPosition;
        Vector2 leftDoorClosedPosition;
        Vector2 rightDoorOpenedPosition;
        Vector2 rightDoorClosedPosition;
        
        #endregion

        #region Inititialization

        public HighscoreBackgroundScreen()
        {
            animateDoors = true;

            if (animateDoors)
            {
                AudioManager.PlaySound("doorOpen");
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            leftDoorTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/left_door");
            rightDoorTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/right_door");

            if (animateDoors)
                doorsInTransition = true;

            leftDoor = new Image(leftDoorTexture);
            rightDoor = new Image(rightDoorTexture);

            leftDoorOpenedPosition = new Vector2(
                -leftDoor.Width(this),
                viewport.Height - leftDoor.Height(this));
            leftDoorClosedPosition = new Vector2(
                0,
                viewport.Height - leftDoor.Height(this));

            rightDoorOpenedPosition = new Vector2(
                viewport.Width, viewport.Height - rightDoor.Height(this));
            rightDoorClosedPosition = new Vector2(
                viewport.Width - rightDoor.Width(this),
                viewport.Height - rightDoor.Height(this));
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, true, coveredByOtherScreen);

            if (doorsInTransition && animateDoors)
                AnimateDoors();    
        }

        private void AnimateDoors()
        {
            if (!doorsHitFinalPosition)
            {
                Vector2 pos = Vector2.Zero;

                pos = leftDoor.Position;
                pos.X = MathHelper.Clamp(
                    leftDoor.Position.X - doorsAnimationStep,
                    leftDoorOpenedPosition.X,
                    leftDoorClosedPosition.X);
                leftDoor.Position = pos;

                pos = rightDoor.Position;
                pos.X = MathHelper.Clamp(
                    pos.X + doorsAnimationStep,
                    rightDoorClosedPosition.X,
                    rightDoorOpenedPosition.X);
                rightDoor.Position = pos;

                if (leftDoor.Position == leftDoorOpenedPosition &&
                    rightDoor.Position == rightDoorOpenedPosition)
                {
                    if (!doorsHitFinalPosition)
                        doorsHitFinalPosition = true;
                    else
                        doorsHitFinalPosition = false;
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw the doors
            spriteBatch.Draw(leftDoor.ImageContents, leftDoor.Position, Color.White);
            spriteBatch.Draw(rightDoor.ImageContents, rightDoor.Position, Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}
