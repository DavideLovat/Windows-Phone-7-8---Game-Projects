using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class TextRows : Text
    {
        #region Fields

        char[] delimeterChars = { ' ', '\n' };
        List<string> textRows = new List<string>();
        int MaxCapacity = 50;
        StringBuilder stringBuilder = new StringBuilder();
        int verticalSpace = 5;

        #endregion

        #region Initialization

        public TextRows(string text, SpriteFont font)
            :base(text, font)
        {
            InitializeTextRows();
        }

        public TextRows(int index, string text, SpriteFont font)
            :base(index, text, font)
        {
            InitializeTextRows();
        }

        #endregion

        #region Methods
        
        public override void Update(GameScreen screen, GameTime gameTime) 
        {
            if (!IsActive)
            {
                return;
            }
        }

        public override void Draw(GameScreen screen, GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }
            
            // Draw textContents, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            Vector2 updatePosition = position;

            for (int i = 0; i < textRows.Count; i++)
            {
                spriteBatch.DrawString(Font, textRows[i], updatePosition, Color.White);
                /*
                Vector2 textSize = Font.MeasureString(textRows[i]);
                updatePosition.Y += textSize.Y + verticalSpace;
                */
                int fontLineSpacing = Font.LineSpacing;
                updatePosition.Y += fontLineSpacing + verticalSpace;
            }
        }

        public virtual int Height()
        {
            float height = 0;

            for (int i = 0; i < textRows.Count; i++)
            {
                /*
                Vector2 rowSize = Font.MeasureString(textRows[i]);
                height = rowSize.Y + verticalSpace;
                */
                int fontLineSpacing = Font.LineSpacing;
                height += fontLineSpacing + verticalSpace;
            }

            return (int)height;
        }

        public virtual int Width()
        {
            float maxWidth = 0;

            for (int i = 0; i < textRows.Count; i++)
            {
                Vector2 rowSize = Font.MeasureString(textRows[i]);
                maxWidth = MathHelper.Max(maxWidth, rowSize.X);
            }

            return (int)maxWidth;
        }

        public virtual void InitializeTextRows()
        {
            string [] splitRows = TextContents.Split(delimeterChars);

            for (int i = 0; i < splitRows.Length; i++)
            {
                if (splitRows.Length > MaxCapacity)
                {
                    string currentRow = splitRows[i];

                    while (currentRow.Length > MaxCapacity)
                    {
                        textRows.Add(currentRow.Substring(0, MaxCapacity));
                        currentRow = currentRow.Substring(MaxCapacity);
                    }

                    if (currentRow.Length > 0)
                    {
                        textRows.Add(currentRow);
                    }
                }
                else
                {
                    textRows.Add(splitRows[i]);
                }
            }
        }

        #endregion

    }
}
