using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class AnswerBar : SideBar
    {
        #region Fields

        List<AnswerTextButton> answerButtons = new List<AnswerTextButton>();

        public List<AnswerTextButton> AnswerButtons
        {
            get { return answerButtons; }
            set { answerButtons = value; }
        }

        public Vector2 answerButtonSpace
        {
            get;
            set;
        }

        #endregion

        #region Initialization

        public AnswerBar(Texture2D outerBlockTexture, Texture2D innerBlockTexture, Texture2D grillBlockTexture,
            Vector2 closedPosition, Vector2 openedPosition, bool sideBarIsOpened, bool grillIsOpened)
            : base(outerBlockTexture, innerBlockTexture, grillBlockTexture,
            closedPosition, openedPosition, sideBarIsOpened, grillIsOpened)
        { }

        public AnswerBar(int index, Texture2D outerBlockTexture, Texture2D innerBlockTexture, Texture2D grillBlockTexture,
            Vector2 closedPosition, Vector2 openedPosition, bool sideBarIsOpened, bool grillIsOpened)
            : base(outerBlockTexture, innerBlockTexture, grillBlockTexture,
            closedPosition, openedPosition, sideBarIsOpened, grillIsOpened)
        { }

        #endregion

        #region Protected Methods

        protected override void UpdateDependingPositionComponent(GameScreen screen, Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.UpdateDependingPositionComponent(screen, gameTime);

            // Answer text buttons position
            Vector2 startedPosition = innerBlock.Position + answerButtonSpace;

            for (int i = 0; i < answerButtons.Count; i++)
            {
                answerButtons[i].Position =
                    startedPosition +
                    new Vector2(
                        0,
                        (answerButtons[i].Height(screen) + 10) * i);
            }
        }

        protected override void DrawContent(GameScreen screen, GameTime gameTime)
        {
            if (answerButtons != null)
            {
                for (int i = 0; i < answerButtons.Count; i++)
                    answerButtons[i].Draw(screen, gameTime);
            }

            base.DrawContent(screen, gameTime);
        }

        #endregion
    }
}
