#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace ZoneGame
{
    public class Player : RiflemanCharacter
    {

        #region Fields

        protected float aimAngle;

        public float AimAngle
        {
            get { return aimAngle; }
        }

        private Texture2D aimLine;

        private Vector2 aimLineCenter;

        private Vector2 aimLineVelocity;

        public Vector2 AimLineVelocity
        {
            get { return aimLineVelocity; }
            set { aimLineVelocity = value; }
        }

        private Vector2 AimLinePosition
        {
            get
            {
                Rectangle playerDim = CurrentFrameDimension;
                return Position + new Vector2(playerDim.Width / 2, 28 - aimLine.Width / 2);//playerDim.Height / 3);
            }
        }

        private SpriteEffects currentEffect = SpriteEffects.None;

        private float aimLineRotation;

        public float AimLineRotation
        {
            get { return aimLineRotation; }
        }                

        public Rectangle ThumbstickArea { get; set; }

        protected BulletManager bulletMang;

        public BulletManager BulletsMang
        {
            get { return bulletMang; }
        }

        private float bulletSpeed = 20f;

        TimeSpan cooldown = TimeSpan.FromSeconds(.37f);//.15f);
        TimeSpan fireTimer;

        #endregion

        #region Bounds

        Vector2 bodySize = new Vector2(78, 78);
        /*
        public override Vector2 HitPosition
        {
            get
            {
                return Position + new Vector2(CurrentFrameCenter.X, 30);
            }
            set 
            {
                Position = value - new Vector2(CurrentFrameCenter.X, 30);
            }
        }
        */
        public override Vector2 HitPoint
        {
            get { return new Vector2(CurrentFrameCenter.X, 30); }
        }

        public Vector2 basePosition
        {
            get
            {
                return Position + new Vector2(
                    CurrentFrameSize.X / 2,
                    65);
            }
        }

        public override Rectangle LayerDepthRectangle
        {
            get
            {
                int offsetX = 26;
                int offsetY = 8;

                int width = 24;
                int height = 61;

                return new Rectangle(
                    (int)Position.X + offsetX,
                    (int)Position.Y + offsetY,
                    width,
                    height);
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                int height = (int)bodySize.Y / 10 * 8;
                int width = (int)bodySize.X / 10 * 5;

                int offsetY = ((int)bodySize.Y - height) / 2;
                int offsetX = ((int)bodySize.X - width) / 2;

                return new Rectangle(
                    (int)Position.X + offsetX,
                    (int)Position.Y + offsetY,
                    width,
                    height);
            }
        }

        public override Rectangle CentralCollisionArea
        {
            get
            {
                Rectangle bounds = Bounds;
                int height = (int)Bounds.Height / 10 * 5;
                int width = (int)Bounds.Width / 10 * 8;

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

        #region Initialization

        public Player(
            AnimatingSprite idleSprite,
            AnimatingSprite walkingSprite,
            AnimatingSprite dyingSprite,
            AnimatingSprite aimSprite,
            Vector2 worldSize,
            Texture2D aimLine, Texture2D bulletTexture)
            : base(idleSprite, walkingSprite, dyingSprite, aimSprite, worldSize)
        {
            CardDirection = CardinalDirection.South;
            AimCardDirection = CardinalDirection.South;
            moveSpeed = 50.0f;
            actorType = ActorType.Human;
            cardinalDirectionOffset = 0.10f;

            this.aimLine = aimLine;
            this.aimLineCenter = new Vector2(aimLine.Width / 2, aimLine.Height);
            this.bulletMang = new BulletManager(bulletTexture, worldSize);            
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime)
        {
            // decrease our fire timer
            fireTimer -= gameTime.ElapsedGameTime;
            bulletMang.Update(gameTime);
            base.Update(gameTime);
        }


        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            if (IsRotating && !IsMoving)
            {
                spriteBatch.Draw(aimLine, AimLinePosition, null, Color.White, aimLineRotation, aimLineCenter, 1f, SpriteEffects.None, 1f);
            }

            bulletMang.Draw(spriteBatch, gameTime);
           
        }

        #endregion

        #region Movement

        protected override void SetMovement(Vector2 movement, GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            direction = CalcolateDirection(movement);
            moveAmount = direction * moveSpeed * elapsedTime;
            position =  position + moveAmount;
        }       

        #region Help Methods Calcolate Movement Direction

        public Vector2 CalcolateDirection(Vector2 movement)
        {
            Vector2 tempVector = Vector2.Zero;

            if (movement != Vector2.Zero)
            {
                if (Math.Abs(movement.X) > Math.Abs(movement.Y))
                {
                    if (movement.X > 0)
                    {
                        if (movement.Y > 0.10f)
                        {
                            tempVector.X = 0.78f;
                            tempVector.Y = 0.78f;
                        }
                        else if (movement.Y < -0.10f)
                        {
                            tempVector.X = 0.78f;
                            tempVector.Y = -0.78f;
                        }
                        else
                        {
                            tempVector.X = 1f;
                            tempVector.Y = 0f;
                        }
                    }
                    else if (movement.X < 0)
                    {
                        if (movement.Y > 0.10f)
                        {
                            tempVector.X = -0.78f;
                            tempVector.Y = 0.78f;
                        }
                        else if (movement.Y < -0.10f)
                        {
                            tempVector.X = -0.78f;
                            tempVector.Y = -0.78f;
                        }
                        else
                        {
                            tempVector.X = -1f;
                            tempVector.Y = 0f;
                        }
                    }
                }
                else
                {
                    if (movement.Y > 0)
                    {
                        if (movement.X > 0.10f)
                        {
                            tempVector.X = 0.78f;
                            tempVector.Y = 0.78f;
                        }
                        else if (movement.X < -0.10f)
                        {
                            tempVector.X = -0.78f;
                            tempVector.Y = 0.78f;
                        }
                        else
                        {
                            tempVector.X = 0f;
                            tempVector.Y = 1f;
                        }
                    }
                    else if (movement.Y < 0)
                    {
                        if (movement.X > 0.10f)
                        {
                            tempVector.X = 0.78f;
                            tempVector.Y = -0.78f;
                        }
                        else if (movement.X < -0.10f)
                        {
                            tempVector.X = -0.78f;
                            tempVector.Y = -0.78f;
                        }
                        else
                        {
                            tempVector.X = 0f;
                            tempVector.Y = -1f;
                        }
                    }
                }
            }
            else
            {
                tempVector = Vector2.Zero;
            }

            return tempVector;
        }

        #endregion

        #endregion

        #region Rotation

        public override void SetRotation(Vector2 movement)
        {
            aimLineRotation = (float)Math.Atan2(movement.Y, movement.X) + MathHelper.PiOver2;
            base.SetRotation(movement);
        }

        #endregion

        #region Handle Bullets

        public void GenerateBullet()
        {
            if (fireTimer <= TimeSpan.Zero)
            {       
                AudioManager.PlaySound("shoot", false, 0.4f);        
                // add a new bullet to our list
                Vector2 bulletVelocity = aimLineVelocity * bulletSpeed;
                bulletMang.GenerateBullet(AimLinePosition, bulletVelocity, Color.Red);

                // reset our timer
                fireTimer = cooldown;
            }
        }

        #endregion
    }
}
