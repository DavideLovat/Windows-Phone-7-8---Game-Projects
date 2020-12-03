using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public class RiflemanCharacter : StandardCharacter
    {
        #region Fields Character Rotation

        public bool HadAimDirectionChanged
        {
            get { return hadAimDirectionChanged; }
        }
        protected bool hadAimDirectionChanged = false;

        public bool IsAiming
        {
            get { return isAiming; }
        }
        protected bool isAiming = false;

        public bool IsRotating
        {
            get { return isRotating; }
            set { isRotating = value; }
        }
        protected bool isRotating = false;

        #endregion

        #region Data

        /// <summary>
        /// The orientation of this object on the map.
        /// </summary>
        private CardinalDirection aimCardDirection;

        /// <summary>
        /// The orientation of this object on the map.
        /// </summary>
        public CardinalDirection AimCardDirection
        {
            get { return aimCardDirection; }
            set { aimCardDirection = value; }
        }

        #endregion

        #region Graphic Data

        /// <summary>
        /// The animating sprite for the aim view of this character.
        /// </summary>
        private AnimatingSprite aimSprite;

        /// <summary>
        /// The animating sprite for the aim view of this character.
        /// </summary>
        public AnimatingSprite AimSprite
        {
            get { return aimSprite; }
            protected set { aimSprite = value; }
        }

        #endregion

        #region Initialization

        public RiflemanCharacter(
            AnimatingSprite idleSprite,
            AnimatingSprite walkingSprite,
            AnimatingSprite dyingSprite,
            AnimatingSprite aimSprite,
            Vector2 worldSize)
            :base(idleSprite,walkingSprite,dyingSprite, worldSize)
        {
            this.aimSprite = aimSprite;
        }

        #endregion

        #region Update and Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State != CharacterState.Aim)
            {
                effects = SpriteEffects.None;
            }
            CurrentAnimatingSprite.Draw(spriteBatch, position, CalcolateLayerDepth(), effects);
        }

        #endregion

        protected override void UpdateCharacterState()
        {
            if (isHit)
            {
                AnimateStateDead();
            }

            if (!IsDeadOrDying)
            {
                if (IsMoving)
                {
                    if (isAiming)
                        isAiming = false;

                    AnimateStateWalking();
                }
                else
                {
                    if (isRotating)
                        AnimateStateAiming();
                    else if(!isAiming)
                        AnimateStateIdle();
                }
            }
        }

        protected override void UpdateDirectionChanged()
        {
            if (!IsDeadOrDying)
            {
                if (hadDirectionChanged)
                {
                    DirectionChanged();

                    hadDirectionChanged = false;
                }

                if (hadAimDirectionChanged)
                {
                    AimDirectionChanged();

                    DirectionChanged();

                    hadAimDirectionChanged = false;
                }
            }
        }

        #region Rotation

        public virtual void SetRotation(Vector2 movement)
        {
            isAiming = true;            

            GetSpriteEffect(movement);

            CardinalDirection tempAimDirection = AimCardDirection;
            tempAimDirection = CalculateAimCardDirection(movement);
            if (tempAimDirection != AimCardDirection)
            {
                hadAimDirectionChanged = true;
                AimCardDirection = tempAimDirection;
                CardDirection = CalculateCardDirectionFromAimDirection(AimCardDirection);
            }
        }

        #region Helps Methods Calcolate Rotation
        private void GetSpriteEffect(Vector2 movementDirection)
        {
            if (movementDirection.X > 0)
            {
                Effects = SpriteEffects.FlipHorizontally;
            }
            else if (movementDirection.X < 0)
            {
                Effects = SpriteEffects.None;
            }
        }

        private CardinalDirection CalculateAimCardDirection(Vector2 movement)
        {
            float angle = -MathHelper.ToDegrees((float)Math.Atan2(movement.Y, movement.X));

            if (angle > 90)
            {
                float diff = Math.Abs(angle) - 90;
                angle = 90 - diff;
            }
            else if (angle < -90)
            {
                float diff = Math.Abs(angle) - 90;
                angle = -(90 - diff);
            }

            if (angle > 0f)
            {
                if (angle < 9f)
                {
                    return CardinalDirection.West;
                }
                else if (angle < 24f)
                {
                    return CardinalDirection.WestNorthwest;
                }
                else if (angle < 36f)
                {
                    return CardinalDirection.NorthWest;
                }
                else if (angle < 55f)
                {
                    return CardinalDirection.NorthNorthwest;
                }
                else
                {
                    return CardinalDirection.North;
                }
            }
            else
            {
                if (angle > -9)
                {
                    return CardinalDirection.West;
                }
                else if (angle > -27)
                {
                    return CardinalDirection.WestSouthwest;
                }
                else if (angle > -42)
                {
                    return CardinalDirection.SouthWest;
                }
                else if (angle > -63)
                {
                    return CardinalDirection.SouthSouthwest;
                }
                else
                {
                    return CardinalDirection.South;
                }
            }
        }

        private Vector2 AngleVector(float angle)
        {
            return new Vector2((float)Math.Cos(MathHelper.ToDegrees(angle)), (float)Math.Sin(MathHelper.ToDegrees(angle)));
        }

        private CardinalDirection CalculateCardDirectionFromAimDirection(CardinalDirection aimDir)
        {
            switch (aimDir)
            {
                case CardinalDirection.South:
                    return CardinalDirection.South;

                case CardinalDirection.SouthSouthwest:
                case CardinalDirection.SouthWest:
                case CardinalDirection.WestSouthwest:
                    return CardinalDirection.SouthWest;

                case CardinalDirection.West:
                    return CardinalDirection.West;

                case CardinalDirection.WestNorthwest:
                case CardinalDirection.NorthWest:
                case CardinalDirection.NorthNorthwest:
                    return CardinalDirection.NorthWest;

                case CardinalDirection.North:
                    return CardinalDirection.North;

                case CardinalDirection.NorthNortheast:
                case CardinalDirection.NorthEast:
                case CardinalDirection.EastNortheast:
                    return CardinalDirection.NorthEast;

                case CardinalDirection.East:
                    return CardinalDirection.East;

                case CardinalDirection.EastSoutheast:
                case CardinalDirection.SouthEast:
                case CardinalDirection.SouthSoutheast:
                    return CardinalDirection.SouthEast;
                default:
                    return CardinalDirection.North;
            }
        }
        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the animations for this character.
        /// </summary>
        public override void ResetAnimation(bool isWalking)
        {
            base.ResetAnimation(isWalking);
            
            if (aimSprite["Aim" + AimCardDirection.ToString()] != null)
            {
                aimSprite.PlayAnimation("Aim", AimCardDirection, true);
            }
            
        }

        public void AnimateStateAiming()
        {
            if (State != CharacterState.Aim)
            {
                State = CharacterState.Aim;
                CurrentAnimatingSprite = AimSprite;
                aimSprite.PlayAnimation("Aim", AimCardDirection, true);
            }
        }

        public virtual void AimDirectionChanged()
        {
            AimSprite.PlayAnimation("Aim", AimCardDirection, true);
        }

        #endregion
    }
}
