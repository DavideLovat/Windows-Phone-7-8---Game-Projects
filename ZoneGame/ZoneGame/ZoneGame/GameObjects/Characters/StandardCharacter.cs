#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace ZoneGame
{
    /// <summary>
    ///  A character in the game world
    /// </summary>
    public class StandardCharacter : Actor
    {
        #region Sounds

        protected string[] strSounds; 

        #endregion

        #region  Fields Character Movement

        public bool IsMoving
        {
            get { return isMoving; }
        }
        protected bool isMoving = false;

        public bool HadDirectionChanged
        {
            get { return HadDirectionChanged; }
        }
        protected bool hadDirectionChanged = false;

        protected float cardinalDirectionOffset = 0.45f;

        #endregion

        #region Bounds

        protected Vector2 moveAmount = Vector2.Zero;

        public Vector2 MoveAmount
        {
            get { return moveAmount; }
        }

        public Vector2 BodySize
        {
            get
            {
                return CurrentFrameSize;
            }
        }

        public virtual Vector2 HitPosition
        {
            get { return Position + HitPoint; }
            set
            {
                Position = value - HitPoint;
            }
        }        

        public virtual Vector2 HitPoint
        {
            get { return new Vector2(CentralCollisionArea.X, CentralCollisionArea.Y); }
        }

        public virtual Rectangle BodyBounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    (int)CurrentFrameSize.X,
                    (int)CurrentFrameSize.Y);
            }
        }

        public virtual Rectangle FrameBounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    (int)CurrentFrameSize.X,
                    (int)CurrentFrameSize.Y);
            }
        }

        public override Rectangle LayerDepthRectangle
        {
            get
            {                
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    Width(),
                    Height());
            }
        }

        public override Rectangle Bounds
        {
            get
            {                
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    (int)CurrentFrameSize.X,
                    (int)CurrentFrameSize.Y);
            }
        }

        public override Rectangle CentralCollisionArea
        {
            get
            {
                Rectangle bounds = Bounds;
                int height = (int)Bounds.Height / 10 * 4;
                int width = (int)Bounds.Width / 10 * 4;

                int offsetY = ((int)Bounds.Height - height) / 2;
                int offsetX = ((int)Bounds.Width - width) / 2;

                return new Rectangle(
                    (int)Bounds.X + offsetX,
                    (int)Bounds.Y + offsetY,
                    width,
                    height);
            }
        }

        public override Rectangle BottomCollisionArea
        {
            get
            {
                Rectangle bounds = Bounds;
                int height = (int)Bounds.Height / 10 * 7;
                int width = (int)Bounds.Width / 10 * 8;

                int offsetY = ((int)Bounds.Height - height);
                int offsetX = ((int)Bounds.Width - width) / 2;

                return new Rectangle(
                    (int)Bounds.X + offsetX,
                    (int)Bounds.Y + offsetY,
                    width,
                    height);
            }
        }

        #endregion

        #region Character State


        /// <summary>
        /// The state of a character
        /// </summary>
        public enum CharacterState
        {
            /// <summary>
            /// Character idle 
            /// </summary>
            Idle,

            /// <summary>
            /// Walking in the world 
            /// </summary>
            Walking,

            /// <summary>
            /// In aim mode
            /// </summary>
            Aim,

            /// <summary>
            /// Performing Attack Animation
            /// </summary>
            Attack,

            /// <summary>
            /// Dead, but still playing the dying animation.
            /// </summary>
            Dying,

            /// <summary>
            /// Dead, with the dead animation
            /// </summary>
            Dead,
        }

        private CharacterState state = CharacterState.Idle;

        public CharacterState State
        {
            get { return state; }
            set { state = value; }
        }

        public bool IsDeadOrDying
        {
            get
            {
                return ((State == CharacterState.Dying) ||
                    (State == CharacterState.Dead));
            }
        }

        public bool IsDead
        {
            get { return State == CharacterState.Dead; }
        }

        public bool IsHit
        {
            get { return isHit; }
            set { isHit = value; }
        }
        protected bool isHit = false;

        #endregion

        #region Data

        private CardinalDirection cardDirection;

        /// <summary>
        /// The orientation of this object on the map.
        /// </summary>
        public CardinalDirection CardDirection
        {
            get { return cardDirection; }
            set { cardDirection = value; }
        }

        public Vector2 CurrentFrameSize
        {
            get
            {
                if(currentAnimatingSprite == null)
                    return Vector2.Zero;
                
                return new Vector2(
                    currentAnimatingSprite.FrameDimensions.X,
                    currentAnimatingSprite.FrameDimensions.Y);

            }
        }

        public Rectangle CurrentFrameDimension
        {
            get 
            {
                if(currentAnimatingSprite == null)
                    return new Rectangle();

                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    currentAnimatingSprite.FrameDimensions.X,
                    currentAnimatingSprite.FrameDimensions.Y);
            }
        }

        public Vector2 CurrentFrameCenter
        {
            get
            {
                if (currentAnimatingSprite == null)
                    return Vector2.Zero;

                return new Vector2(
                    currentAnimatingSprite.FrameDimensions.X / 2,
                    currentAnimatingSprite.FrameDimensions.Y / 2);
            }
        }

        #endregion

        #region Graphics Data

        private AnimatingSprite currentAnimatingSprite = null;

        public AnimatingSprite CurrentAnimatingSprite
        {
            get { return currentAnimatingSprite; }
            protected set { currentAnimatingSprite = value; }
        }


        private AnimatingSprite idleSprite;

        public AnimatingSprite IdleSprite
        {
            get { return idleSprite; }
            protected set { idleSprite = value; }
        }

        private AnimatingSprite walkingSprite;

        public AnimatingSprite WalkingSprite
        {
            get { return walkingSprite; }
            protected set { walkingSprite = value; }
        }

        private AnimatingSprite dyingSprite;

        public AnimatingSprite DyingSprite
        {
            get { return dyingSprite; }
            protected set { dyingSprite = value; }
        }

        #endregion

        #region Initialization

        public StandardCharacter(
            AnimatingSprite idleSprite,
            AnimatingSprite walkingSprite,
            AnimatingSprite dyingSprite,
            Vector2 worldSize)
            :base(worldSize)
        {
            this.IdleSprite = idleSprite;
            this.WalkingSprite = walkingSprite;
            this.dyingSprite = dyingSprite;                       
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime)
        {
            UpdateCharacterState();
            UpdateDirectionChanged();

            UpdateAnimation(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            CurrentAnimatingSprite.Draw(spriteBatch, position, CalcolateLayerDepth(), effects);
        }

        #endregion

        protected virtual void UpdateCharacterState()
        {
            if (isHit)
            {
                AnimateStateDead();
            }

            if (!IsDeadOrDying)
            {
                if (IsMoving)
                {
                    AnimateStateWalking();
                }
                else
                {
                    AnimateStateIdle();
                }
            }
        }

        protected virtual void UpdateDirectionChanged()
        {
            if (!IsDeadOrDying)
            {
                if (hadDirectionChanged)
                {
                    DirectionChanged();

                    hadDirectionChanged = false;
                }
            }
        }

        #region Movement
        /// <summary>
        /// Move the character by the given amount.
        /// </summary>
        public virtual void Move(Vector2 movement, bool move, GameTime gameTime)
        {
            isMoving = move;

            SetMovement(movement, gameTime);

            // if the position is moving, up the direction
            if (IsMoving)
            {
                CardinalDirection tempDirection = CardDirection;
                tempDirection = CalcolateCardinalDirection(movement);
                if (tempDirection != CardDirection)
                {
                    hadDirectionChanged = true;
                    CardDirection = tempDirection;
                }
            }
        }

        protected virtual void SetMovement(Vector2 movement, GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            moveAmount = direction * moveSpeed * elapsedTime;
            position = position + moveAmount;
        }

        #region Help Methods Calcolate Cardinal Direction
        /// <summary>
        /// Determine the direction based on the given movement vector.
        /// </summary>
        /// <param name="vector">The vector that the player is moving.</param>
        /// <returns>The calculated direction.</returns>
        protected CardinalDirection CalcolateCardinalDirection(Vector2 vector)
        {
            float offset = cardinalDirectionOffset;
            if (Math.Abs(vector.X) > Math.Abs(vector.Y))
            {
                return DeterminateCardDirectionDomainX(vector, offset);
            }
            else
            {
                return DeterminateCardDirectionDomainY(vector, offset);
            }
        }

        private CardinalDirection DeterminateCardDirectionDomainX(Vector2 vector, float offset)
        {
            if (vector.X > 0)
            {
                if (vector.Y > offset)
                {
                    return CardinalDirection.SouthEast;
                }
                else if (vector.Y < -offset)
                {
                    return CardinalDirection.NorthEast;
                }
                else
                {
                    return CardinalDirection.East;
                }
            }
            else
            {
                if (vector.Y > offset)
                {
                    return CardinalDirection.SouthWest;
                }
                else if (vector.Y < -offset)
                {
                    return CardinalDirection.NorthWest;
                }
                else
                {
                    return CardinalDirection.West;
                }
            }
        }

        private CardinalDirection DeterminateCardDirectionDomainY(Vector2 vector, float offset)
        {
            if (vector.Y > 0)
            {
                if (vector.X > offset)
                {
                    return CardinalDirection.SouthEast;
                }
                else if (vector.X < -offset)
                {
                    return CardinalDirection.SouthWest;
                }
                else
                {
                    return CardinalDirection.South;
                }
            }
            else
            {
                if (vector.X > offset/* 0.10f*/)
                {
                    return CardinalDirection.NorthEast;
                }
                else if (vector.X < -offset)
                {
                    return CardinalDirection.NorthWest;
                }
                else
                {
                    return CardinalDirection.North;
                }
            }
        }
        #endregion

        #endregion

        #region Public Methods
       
        public override int Width()
        {
            if(currentAnimatingSprite != null)
                return currentAnimatingSprite.FrameDimensions.X;

            return base.Width();
        }


        public override int Height()
        {
            if(currentAnimatingSprite != null)
            return currentAnimatingSprite.FrameDimensions.Y;

            return base.Height();
        }

        /// <summary>
        /// Reset the animations for this character.
        /// </summary>
        public virtual void ResetAnimation(bool isWalking)
        {
            state = isWalking ? CharacterState.Walking : CharacterState.Idle;

            switch (state)
            {
                case CharacterState.Idle:
                    currentAnimatingSprite = idleSprite;
                    break;
                case CharacterState.Walking:
                    currentAnimatingSprite = walkingSprite;
                    break;
            }
            
            if(idleSprite["Idle" + CardDirection.ToString()] != null)
                idleSprite.PlayAnimation("Idle", CardDirection, true);
            
            if (walkingSprite["Walk" + CardDirection.ToString()] != null)
                walkingSprite.PlayAnimation("Walk", CardDirection, true);
            
            if (dyingSprite["Die" + CardDirection.ToString()] != null)
                dyingSprite.PlayAnimation("Die", CardDirection, true);
        }

        #endregion

        #region Protected Methods

        public void AnimateStateIdle()
        {
            if (State != CharacterState.Idle)
            {
                State = CharacterState.Idle;
                CurrentAnimatingSprite = IdleSprite;               
                IdleSprite.PlayAnimation("Idle", CardDirection, true);
            }
        }

        public void AnimateStateWalking()
        {
            if (State != CharacterState.Walking)
            {
                State = CharacterState.Walking;
                CurrentAnimatingSprite = WalkingSprite;
                WalkingSprite.PlayAnimation("Walk", CardDirection, true);
            }
        }

        public void AnimateStateDead()
        {
            if (!IsDead)
            {
                if (!IsDeadOrDying)
                {
                    State = CharacterState.Dying;
                    CurrentAnimatingSprite = DyingSprite;
                    DyingSprite.PlayAnimation("Die", CardDirection, true);
                }

                if (State == CharacterState.Dying)
                {
                    if (DyingSprite.IsPlaybackComplete)
                    {
                        State = CharacterState.Dead;
                    }
                }
            }
        }

        public virtual void DirectionChanged()
        {
            if (State != CharacterState.Walking)
            {
                WalkingSprite.PlayAnimation("Walk", CardDirection, true);
            }
            else
            {
                WalkingSprite.PlayAnimation("Walk", CardDirection, false);
            }
            
            IdleSprite.PlayAnimation("Idle", CardDirection, true);

            DyingSprite.PlayAnimation("Die", CardDirection, true);
        }

        public virtual void UpdateAnimation(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentAnimatingSprite.UpdateAnimation(elapsedSeconds);
        }

        #endregion
        
        #region LayerDepth Methods

        protected virtual float CalcolateLayerDepth()
        {
            float layer = MathHelper.Clamp(((LayerDepthRectangle.Y + LayerDepthRectangle.Height) / worldSize.Y), 0f, 1f);
            return layer;
        }

        #endregion

        #region Helps Sounds Methods
        
        protected void PlaySound(string strSound)
        {
            if (strSounds != null)
            {
                if (!String.IsNullOrEmpty(strSound))
                {
                    for (int i = 0; i < strSounds.Length; i++)
                    {
                        if (strSounds[i].CompareTo(strSound) == 0)
                        {
                            continue;
                        }

                        AudioManager.StopSound(strSounds[i]);
                    }

                    AudioManager.PlaySound(strSound);
                }
            }
        }

        #endregion
    }
}
