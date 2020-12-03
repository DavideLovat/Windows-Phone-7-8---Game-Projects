using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class QuestionBar : SideBar
    {
        #region Fields

        Paragraph questionParagraph;

        public Paragraph QuestionParagraph
        {
            get { return questionParagraph; }
            set { questionParagraph = value; }
        }

        public Vector2 paragraphSpace
        {
            get;
            set;
        }

        #endregion

        #region Initialization

        public QuestionBar(Texture2D outerBlockTexture, Texture2D innerBlockTexture, Texture2D grillBlockTexture,
            Vector2 closedPosition, Vector2 openedPosition, bool sideBarIsOpened, bool grillIsOpened)
            : base(outerBlockTexture, innerBlockTexture, grillBlockTexture,
            closedPosition, openedPosition, sideBarIsOpened, grillIsOpened)
        { }

        public QuestionBar(int index, Texture2D outerBlockTexture, Texture2D innerBlockTexture, Texture2D grillBlockTexture,
            Vector2 closedPosition, Vector2 openedPosition, bool sideBarIsOpened, bool grillIsOpened)
            : base(outerBlockTexture, innerBlockTexture, grillBlockTexture,
            closedPosition, openedPosition, sideBarIsOpened, grillIsOpened)
        { }

        #endregion


        #region Protected Methods

        protected override void UpdateDependingPositionComponent(GameScreen screen, GameTime gameTime)
        {
            base.UpdateDependingPositionComponent(screen, gameTime);
            
            if (questionParagraph != null)
            {
                questionParagraph.Position = innerBlock.Position + paragraphSpace;
            }

        }

        protected override void DrawContent(GameScreen screen, GameTime gameTime)
        {
            if(questionParagraph != null)
                questionParagraph.Draw(screen, gameTime);

            base.DrawContent(screen, gameTime);
        }

        #endregion
    }
}
