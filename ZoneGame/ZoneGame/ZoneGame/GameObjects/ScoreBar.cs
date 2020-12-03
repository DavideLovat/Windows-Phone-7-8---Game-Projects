using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class ScoreBar : InGameComponent
    {
        #region Enums


        /// <summary>
        /// Used to determine the component's orientation.
        /// </summary>
        public enum ScoreBarOrientation
        {
            Vertical,
            Horizontal
        }


        #endregion

        #region Fields/Properties


        public int MinValue { get; set; }
        public int MaxValue { get; set; }        
        public Color ScoreBarColor { get; set; }
        public int CurrentValue
        {
            get
            {
                return currentValue;
            }
        }

        public bool isMaxCurrentValue
        {
            get { return currentValue == MaxValue; }
        }

        ScoreBarOrientation scoreBarOrientation;
        int height;
        int width;
        
        int currentValue;

        Texture2D backgroundTexture;
        Texture2D redTexture;
        Texture2D greenTexture;
        Texture2D yellowTexture;

        #endregion

        #region Initialization


        /// <summary>
        /// Creates a new score bar instance.
        /// </summary>
        /// <param name="game">The associated game object.</param>
        /// <param name="minValue">The score bar's minimal value.</param>
        /// <param name="maxValue">The score bar's maximal value.</param>
        /// <param name="position">The score bar's position.</param>
        /// <param name="height">The score bar's height.</param>
        /// <param name="width">The score bar's width.</param>
        /// <param name="scoreBarColor">Color to tint the scorebar's background with.</param>
        /// <param name="scoreBarOrientation">The score bar's orientation.</param>
        /// <param name="initialValue">The score bar's initial value.</param>
        /// <param name="screen">Gameplay screen where the score bar will appear.</param>
        /// <param name="isAppearAtCountDown">Whether or not the score bar will appear during the game's initial
        /// countdown phase.</param>
        public ScoreBar(Texture2D backgroundTexture, Texture2D greenTexture, Texture2D yellowTexture, Texture2D redTexture,
            int minValue, int maxValue, int height, int width,
            Color scoreBarColor, ScoreBarOrientation scoreBarOrientation, int initialValue)
            : base()
        {
            this.backgroundTexture = backgroundTexture;
            this.greenTexture = greenTexture;
            this.yellowTexture = yellowTexture;
            this.redTexture = redTexture;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Position = position;
            this.ScoreBarColor = scoreBarColor;
            this.scoreBarOrientation = scoreBarOrientation;
            this.currentValue = initialValue;
            this.width = width;
            this.height = height;            

            GetSpaceFromBoarder();
        }
        /*
        /// <summary>
        /// Loads the content that this component will use.
        /// </summary>
        protected override void LoadContent()
        {
            backgroundTexture = Game.Content.Load<Texture2D>("Textures/barBlackBorder");
            greenTexture = Game.Content.Load<Texture2D>("Textures/barGreen");
            yellowTexture = Game.Content.Load<Texture2D>("Textures/barYellow");
            redTexture = Game.Content.Load<Texture2D>("Textures/barRed");
        }
        */

        #endregion

        #region Render


        /// <summary>
        /// Draws the component.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {            
            float rotation;
            // Determine the orientation of the component
            if (scoreBarOrientation == ScoreBar.ScoreBarOrientation.Horizontal)
            {
                rotation = 0f;
            }
            else
            {
                rotation = 1.57f;
            }

            // Draws the background of the score bar
            spriteBatch.Draw(backgroundTexture, new Rectangle((int)Position.X, (int)Position.Y, width, height),
                null, ScoreBarColor, rotation, new Vector2(0, 0), SpriteEffects.None, 0f);

            // Gets the margin from the border
            decimal spaceFromBorder = GetSpaceFromBoarder();

            spaceFromBorder += 4;
            Texture2D coloredTexture = GetTextureByCurrentValue(currentValue);

            if (scoreBarOrientation == ScoreBarOrientation.Horizontal)
            {

                spriteBatch.Draw(coloredTexture, new Rectangle((int)Position.X + 2, (int)Position.Y + 2,
                    width - (int)spaceFromBorder, height - 4), null, Color.White, rotation, new Vector2(0, 0),
                    SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(coloredTexture, new Rectangle((int)Position.X + 2 - height,
                    (int)Position.Y + width + -2, width - (int)spaceFromBorder, height - 4), null, ScoreBarColor,
                    -rotation, new Vector2(0, 0), SpriteEffects.None, 0f);
            }

            base.Draw(spriteBatch, gameTime);
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Increases the current value of the score bar.
        /// </summary>
        /// <param name="valueToIncrease">Number to increase the value by</param>
        /// <remarks>Negative numbers will have no effect.</remarks>
        public void IncreaseCurrentValue(int valueToIncrease)
        {
            // Make sure that the target value does not exceed the max value
            if (valueToIncrease >= 0 && currentValue < MaxValue && currentValue + valueToIncrease <= MaxValue)
            {
                currentValue += valueToIncrease;
            }
            // If the target value exceeds the max value, clamp the value to the maximum
            else if (currentValue + valueToIncrease > MaxValue)
            {
                currentValue = MaxValue;
            }
        }

        /// <summary>
        /// Decreases the current value of the score bar.
        /// </summary>
        /// <param name="valueToDecrease">Number to decrease the value by.</param>
        public void DecreaseCurrentValue(int valueToDecrease)
        {
            DecreaseCurrentValue(valueToDecrease, false);
        }

        /// <summary>
        /// Decreases the current value of the score bar.
        /// </summary>
        /// <param name="valueToDecrease">Number to decrease the value by.</param>
        /// <param name="clampToMinimum">If true, then decreasing by an amount which will cause the bar's value
        /// to go under its minimum will set the bar to its minimal value. If false, performing such an operation
        /// as just described will leave the bar's value unchanged.</param>
        /// <returns>The actual amount which was subtracted from the bar's value.</returns>
        public int DecreaseCurrentValue(int valueToDecrease, bool clampToMinimum)
        {
            // Make sure that the target value does not exceed the min value
            int valueThatWasDecreased = 0;
            if (valueToDecrease >= 0 && currentValue > MinValue && currentValue - valueToDecrease >= MinValue)
            {
                currentValue -= valueToDecrease;
                valueThatWasDecreased = valueToDecrease;
            }
            // If the target value exceeds the min value, clamp the value to the minimum (or do nothing)
            else if (currentValue - valueToDecrease < MinValue && clampToMinimum)
            {
                valueThatWasDecreased = currentValue - MinValue;
                currentValue = MinValue;
            }

            return valueThatWasDecreased;
        }


        #endregion

        #region Private Methods


        /// <summary>
        /// Calculate the empty portion of the score bar according to its current value.
        /// </summary>
        /// <returns>Width of the bar portion which should be empty to represent the bar's current score.</returns>
        private decimal GetSpaceFromBoarder()
        {
            int textureSize;
            textureSize = width;

            decimal valuePercent = Decimal.Divide(currentValue, MaxValue) * 100;
            return textureSize - ((decimal)textureSize * valuePercent / (decimal)100);
        }

        /// <summary>
        /// Returns a texture for the score bar's "fill" according to its value.
        /// </summary>
        /// <param name="value">Current value of the score bar.</param>
        /// <returns>A texture the color of which indicates how close to the bar's maximum the value is.</returns>
        private Texture2D GetTextureByCurrentValue(int value)
        {
            Texture2D selectedTexture;

            decimal valuePercent = Decimal.Divide(currentValue, MaxValue) * 100;
            if (valuePercent > 50)
            {
                selectedTexture = greenTexture;
            }
            else if (valuePercent > 25)
            {
                selectedTexture = yellowTexture;
            }
            else
            {
                selectedTexture = redTexture;
            }
            return selectedTexture;
        }


        #endregion

        #region Public Methods

        public override int Width()
        {
            return width;
        }

        public override int Height()
        {
            return height;
        }

        #endregion
    }
}
