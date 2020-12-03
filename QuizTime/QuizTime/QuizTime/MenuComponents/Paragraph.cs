using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    class Paragraph : Text
    {
        #region Fields

        char[] delimeterChars = {' ', '\n' };
        List<string> textRows = new List<string>();
        int capacity = 20;
        int rowWidth = 380;
        int verticalSpace = 5;

        #endregion

        #region Properties

        public override string TextContents
        {
            get
            {
                return base.TextContents;
            }
            set
            {
                base.TextContents = value;
                InitializeTextRows();
            }
        }

        public List<string> TextRows
        {
            get { return textRows; }
        }

        public char[] DelimeterChars
        {
            get { return delimeterChars; }
            set { delimeterChars = value; }
        }

        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }

        public int RowWidth
        {
            get { return rowWidth; }
            set { rowWidth = value; }
        }

        public int VerticalSpace
        {
            get { return verticalSpace; }
            set { verticalSpace = value; }
        }

        #endregion

        #region Initialization

        public Paragraph(string text, SpriteFont font)
            :base(text, font)
        {
            InitializeTextRows();
            //CreateParagraph();
        }

        public Paragraph(int index, string text, SpriteFont font)
            :base(index, text, font)
        {
            InitializeTextRows();
            //CreateParagraph();
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

        private void CreateParagraph()
        {
            string str = TextContents;

            textRows.Clear();

            while (str.Length > capacity)
            {
                string subString = str.Substring(0, capacity);
                str = str.Substring(capacity, (str.Length - capacity));
                textRows.Add(subString);
            }

            if (!String.IsNullOrEmpty(str))
                textRows.Add(str);
        }

        /*
        private void CreateParagraph()
        {            
            string str = TextContents;
            char find = ' ';

            str = RemoveStartSpace(str);
            str = RemoveEndSpace(str);

            while (str.Length > capacity)
            {
                
                int firstFind;
                int lastFind;

                string subString = str.Substring(0, capacity);

                if (!String.IsNullOrEmpty(subString))
                {
                    firstFind = subString.LastIndexOf(find);
                    if (firstFind != -1 && firstFind != 0)
                    {
                        lastFind = str.IndexOf(find, firstFind);
                        int lengthCutWord = lastFind - firstFind;

                        if (lengthCutWord > capacity)
                        {
                            str = str.Substring(capacity, (str.Length - capacity));
                            textRows.Add(subString);
                        }
                        else if(lengthCutWord + (1 + firstFind) > capacity)
                        {
                            subString = str.Substring(firstFind, );
                            str = str.Substring(0, firstFind + 1);
                            textRows.Add
                        }
                    }
                    else
                    {
                        str = str.Substring(capacity, (str.Length - capacity));
                        textRows.Add(subString);
                    }

                    string cutWord = str.Substring(firstFind, lengthCutWord);

                }
                
            }

            if (!String.IsNullOrEmpty(str))
                textRows.Add(str);

        }
    
   
        private string RemoveStartSpace(string str)
        {
            char find = ' ';

            while (str.IndexOf(find) == 0)
            {
                str = str.Remove(0);
            }

            return str;
        }

        private string RemoveEndSpace(string str)
        {
            char find = ' ';

            while (str.LastIndexOf(find) == str.Length - 1)
            {
                str = str.Remove(str.Length - 1);
            }

            return str;
        }
        */
        
        public virtual void InitializeTextRows()
        {
            TextRows.Clear();

            if (Font.MeasureString(TextContents).X > rowWidth)
            {
                string[] splitRows = TextContents.Split(delimeterChars);
                List<StringBuilder> words = new List<StringBuilder>();
                StringBuilder appendWord = new StringBuilder();
                string word;
                StringBuilder lastWord = new StringBuilder();
                for (int i = 0; i < splitRows.Length; i++)
                {
                    word = splitRows[i] + " ";

                    if (Font.MeasureString(word).X > rowWidth)
                    {
                        if (!String.IsNullOrEmpty(appendWord.ToString()))
                        {
                            if (Font.MeasureString(appendWord.ToString()).X >= rowWidth)
                            {
                                words.Add(appendWord);
                                appendWord = new StringBuilder();
                            }
                        }

                        char[] characters = word.ToString().ToCharArray();
                        //StringBuilder newWord = new StringBuilder();

                        for (int j = 0; j < characters.Length; j++)
                        {
                            appendWord.Append(characters[j]);

                            if (Font.MeasureString(appendWord).X >= rowWidth)
                            {
                                //words.Add(newWord);
                                words.Add(appendWord);
                                appendWord = new StringBuilder();
                            }
                        }

                        if (!(i < splitRows.Length - 1))
                        {
                            words.Add(appendWord);
                        }
                        /*
                        if (!String.IsNullOrEmpty(newWord.ToString()))
                        {
                            lastWord = newWord;
                        }
                        */
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(appendWord.ToString()))
                        {
                            appendWord.Append(word);
                            continue;
                        }

                        StringBuilder prova = new StringBuilder();
                        prova.Append(appendWord);
                        prova.Append(word);
                        if (Font.MeasureString(prova).X < rowWidth)
                        {
                            appendWord.Append(word);
                            if (!(i < splitRows.Length - 1))
                            {
                                words.Add(appendWord);
                            }
                        }
                        else
                        {
                            words.Add(appendWord);
                            if (i < splitRows.Length - 1)
                            {
                                appendWord = new StringBuilder();
                                appendWord.Append(word);
                            }
                            else
                            {
                                words.Add(new StringBuilder(word));

                            }
                        }
                    }

                }

                foreach (StringBuilder stringBuilder in words)
                {
                    textRows.Add(stringBuilder.ToString());
                }
            }
            else
            {
                textRows.Add(TextContents);
            }
        }

        private float MisureTextRowLength()
        {
            if (textRows.Count > 0)
                return Font.MeasureString(textRows[textRows.Count - 1]).X;
            else
                return 0;
        }

        #endregion
    }
}
