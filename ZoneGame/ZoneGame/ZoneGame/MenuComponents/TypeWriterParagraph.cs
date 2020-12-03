using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class TypeWriterParagraph : Paragraph
    {
        public bool IsDoneDrawing
        {
            get { return isDoneDrawing; }
            set { isDoneDrawing = value; }
        }
        bool isDoneDrawing;

        public bool IsNext
        {
            get { return isNext; }
            set { isNext = value; }
        }
        bool isNext;

        public bool TypeWriterIsIdle
        {
            get;
            protected set;
        }

        public override Rectangle ParagraphBounds
        {
            get
            {
                return base.ParagraphBounds;
            }
            set
            {
                base.ParagraphBounds = value;
                Reset(text);
            }
        }

        public override SpriteFont Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                Reset(text);
            }
        }

        public override string Text
        {
            get
            {                
                return base.Text;
            }
            set
            {                
                base.Text = value;
                Reset(text);
            }
        }

        List<String> stringToDisplay;
        String typedText;
        int indexList;
        int delayInMilliseconds;
        double typedTextLength;
        
        public TypeWriterParagraph(SpriteFont font)
            : this(0, font, Rectangle.Empty, String.Empty)
        {            
        }

        public TypeWriterParagraph(SpriteFont font, Rectangle bounds, String text)
            : this(0, font, bounds, text)
        {
        }

        public TypeWriterParagraph(int index, SpriteFont font)
            : this(index, font, Rectangle.Empty, String.Empty)
        {            
        }

        public TypeWriterParagraph(int index, SpriteFont font, Rectangle bounds, String text)
            :base(index, font, bounds, text)
        {
            delayInMilliseconds = 50;
            Reset(text);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            if (stringToDisplay != null && stringToDisplay.Count > 0)
            {
                if (isNext || indexList > stringToDisplay.Count - 1)
                {
                    TypeWriterIsIdle = false;
                    if (indexList < stringToDisplay.Count - 1)
                    {
                        indexList++;
                        InitializeNext();
                    }
                    else
                    {
                        ResetFields();
                    }
                }

                if (delayInMilliseconds == 0)
                {
                    if (indexList < stringToDisplay.Count - 1)
                    {
                        isDoneDrawing = true;
                    }
                    TypeWriterIsIdle = true;
                    typedText = stringToDisplay[indexList];
                }
                else if (typedTextLength < stringToDisplay[indexList].Length)
                {
                    typedTextLength = typedTextLength + gameTime.ElapsedGameTime.TotalMilliseconds / delayInMilliseconds;

                    if (typedTextLength >= stringToDisplay[indexList].Length)
                    {
                        typedTextLength = stringToDisplay[indexList].Length;
                        TypeWriterIsIdle = true;
                        if (indexList < stringToDisplay.Count - 1)
                        {
                            //isNext = true;
                        }
                        else
                        {
                            isDoneDrawing = true;
                        }
                    }

                    typedText = stringToDisplay[indexList].Substring(0, (int)typedTextLength);
                }
            }
            else
            {
                typedText = String.Empty;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            if (typedText != null)
                spriteBatch.DrawString(font, typedText, new Vector2(paragraphBounds.X, paragraphBounds.Y), Color);            
        }

        #region Protected Methods

        protected virtual void Reset(String text)
        {            
            ResetSplitParsedText();        
            ResetFields();
        }

        protected virtual List<String> SplitParsedText()
        {
            String parsedText = parseText(text);
            String line = String.Empty;            
            String[] stringArray = parsedText.Split('\n');
            String returnString = String.Empty;
            List<String> stringList = new List<string>();

            foreach (String str in stringArray)
            {
                if (font.MeasureString(line + str).Y > paragraphBounds.Height)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        stringList.Add(line);
                    }
                    line = String.Empty;
                }

                line = line + str + '\n';
            }

            stringList.Add(line);

            return stringList;
        }

        public void ResetSplitParsedText()
        {
            stringToDisplay = SplitParsedText();
        }

        public void ResetFields()
        {
            RestartParagraph();
            InitializeNext();
        }

        protected void RestartParagraph()
        {
            indexList = 0;
            isDoneDrawing = false;
        }

        protected void InitializeNext()
        {
            typedText = String.Empty;
            typedTextLength = 0;
            isNext = false;
        }

        #endregion
    }
}
