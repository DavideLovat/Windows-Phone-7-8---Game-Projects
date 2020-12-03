using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class TextBox : Component
    {
        Texture2D portraitTexture, textBoxTexture, arrowTexture;
        SpriteFont font;

        Image portrait;
        Image textBox;
        Image arrow;
        TypeWriterParagraph typeWriter;
        
        Rectangle writerBounds;
        int padding = 20;

        public new Vector2 Position
        {
            get { return position; }
            set 
            { 
                position = value;
                UpdatePosition();
            }
        }

        public bool MoveNext
        {
            get;
            set;
        }

        public bool TypeWriterIsIdle
        {
            get { return typeWriter.TypeWriterIsIdle; }
        }

        public Rectangle Bounds
        {
            get { return Rectangle.Empty; }
        }

        public bool IsDialogEnded
        {
            get {return typeWriter.IsDoneDrawing;}            
        }        

        public String Text
        {
            get { return typeWriter.Text; }
            set { typeWriter.Text = value; }
        }

        public TextBox(Texture2D portraitText, Texture2D boxText, Texture2D arrowText, SpriteFont font)
        {
            this.portraitTexture = portraitText;
            this.textBoxTexture = boxText;
            this.arrowTexture = arrowText;
            this.font = font;

            portrait = new Image(portraitTexture);
            textBox = new Image(textBoxTexture);
            arrow = new Image(arrowTexture);
            typeWriter = new TypeWriterParagraph(font);
            typeWriter.ParagraphBounds = new Rectangle((int)Position.X, (int)Position.Y, 475, 100);
        }

        public override void Update(GameTime gameTime)
        {
            if (MoveNext)
            {                
                typeWriter.IsNext = true;
                MoveNext = false;
            }                

            typeWriter.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            textBox.Draw(spriteBatch, gameTime);
            portrait.Draw(spriteBatch, gameTime);
            typeWriter.Draw(spriteBatch, gameTime);
            if (!typeWriter.IsDoneDrawing && typeWriter.TypeWriterIsIdle)
                arrow.Draw(spriteBatch, gameTime);
        }

        public override int Width()
        {
            return textBox.Width();
        }

        public override int Height()
        {
            return Math.Max(textBox.Height(), portrait.Height());
        }

        public virtual void UpdatePosition()
        {
            portrait.Position = position;
            textBox.Position =
                portrait.Position +
                new Vector2(30, portrait.Height() - textBox.Height());
                /*new Vector2(
                    ((portrait.Width() / 10) * 2),
                    portrait.Position.Y + portrait.Height() - textBox.Height());*/
            typeWriter.Position =
                new Vector2(portrait.Position.X + portrait.Width() + padding, textBox.Position.Y + padding);                
            arrow.Position = new Vector2(
                textBox.Position.X + 
                textBox.Width() -
                padding,
                textBox.Position.Y +
                textBox.Height() -
                padding / 2);
        }
    }
}
