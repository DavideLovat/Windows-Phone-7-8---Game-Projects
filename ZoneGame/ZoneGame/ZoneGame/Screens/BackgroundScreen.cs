using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ZoneGame
{
    class BackgroundScreen : GameScreen
    {
        #region Fields

        Texture2D backgroundTexture;
        string backgroundName;

        #endregion

        #region Initialization

        public BackgroundScreen(string backgroundName)
        {
            this.backgroundName = backgroundName;
        }

        public override void LoadContent()
        {
            if (contentDynamic == null)
                contentDynamic = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = contentDynamic.Load<Texture2D>("Textures/Backgrounds/" + backgroundName);
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            /*Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            */
            spriteBatch.Begin();

            //spriteBatch.Draw(backgroundTexture, fullscreen, Color.White);
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}
