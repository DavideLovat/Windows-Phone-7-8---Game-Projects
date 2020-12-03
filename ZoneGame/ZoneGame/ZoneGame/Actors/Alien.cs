using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class Alien : MonsterCharacter
    {
        #region Debug        

        bool debugVisible = false;

        #endregion

        #region Bounds

        public override Rectangle BodyBounds
        {
            get
            {
                return new Rectangle
                    (
                        (int)Position.X + 14,
                        (int)Position.Y + 4,
                        52, 
                        48);
            }
        }
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
            get
            {
                return new Vector2(CurrentFrameCenter.X, 30);
            }
        }

        #endregion

        #region DebugFields

        bool playerIsHit = false;

        #endregion

        #region Fields

        float attackRadius;

        protected Random random;
        Vector2 aiNewDir;
        int aiNumSeen;
        int boundryWidth = 800;
        int boundryHeight = 480;
        Player playerActor;
        public bool preyReached = false;

        public bool IsIndisposed
        {
            get { return IsDeadOrDying || IsHit; }
        }

        public bool RemoveAlien
        {
            get { return removeAlien; }
            set { removeAlien = value; }
        }
        private bool removeAlien = false;

        TimeSpan timeToRemove = TimeSpan.FromSeconds(10);

        TimeSpan attackTimer = TimeSpan.FromSeconds(0.2);
        TimeSpan attackTimeElapsed = TimeSpan.Zero;

        #endregion

        #region Initialization

        public Alien(
            AnimatingSprite idleSprite,
            AnimatingSprite walkingSprite,
            AnimatingSprite dyingSprite,
            AnimatingSprite attackSprite,
            Vector2 worldSize, 
            Player playerActor)
            : base(idleSprite, walkingSprite, dyingSprite, attackSprite, worldSize)
        {        
            strSounds = new string[]{ "enemyDead", "enemyAttack" };
            direction = Vector2.Zero;
            if(direction != Vector2.Zero)
                direction.Normalize();
            //position = loc;
            moveSpeed = 60.0f;
            //random = new Random((int)loc.X + (int)loc.Y);
            random = new Random();
            actorType = ActorType.Alien;
            this.playerActor = playerActor;
            BuildBehaviors();
            Point frameDimension = attackSprite.FrameDimensions;
            attackRadius = (float)Math.Sqrt(frameDimension.X * frameDimension.X + frameDimension.Y * frameDimension.Y) * .45f;

        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// update bird position, wrapping around the screen edges
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime, ref AIParameters aiParams)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsDead)
            {
                if (timeToRemove <= TimeSpan.Zero)
                {
                    removeAlien = true;
                }
                else
                {
                    timeToRemove -= gameTime.ElapsedGameTime;
                }
            }
            
            if(attackTimeElapsed > TimeSpan.Zero)
                attackTimeElapsed -= gameTime.ElapsedGameTime;
            
            Vector2 randomDir = Vector2.Zero;

            randomDir.X = (float)random.NextDouble() - 0.5f;
            randomDir.Y = (float)random.NextDouble() - 0.5f;
            Vector2.Normalize(ref randomDir, out randomDir);
            
            if(IsAttacking && !IsIndisposed)
            {
                if (State == CharacterState.Attack)
                {
                    if (CurrentAnimatingSprite.CurrentRowFrame >= 3)
                    {
                        if (IsPlayerHit())
                        {
                            playerIsHit = true;
                            PlaySound("enemyAttack");
                            //playerActor.IsHit = true;
                        }
                        else
                            playerIsHit = false;
                    }

                    if (CurrentAnimatingSprite.IsPlaybackComplete)
                    {
                        isAttacking = false;
                        attackTimeElapsed = attackTimer;
                        Vector2 playerDirection = playerActor.HitPosition - HitPosition;
                        if (playerDirection != Vector2.Zero)
                            playerDirection.Normalize();
                        CardinalDirection tempDirection = CalcolateCardinalDirection(playerDirection);
                        if (tempDirection != CardDirection)
                        {
                            hadDirectionChanged = true;
                            CardDirection = tempDirection;
                        }
                        AttackSprite.ResetAnimation();
                    }
                }
                else
                {
                    isAttacking = false;
                }
            }

            if (!playerActor.CentralCollisionArea.Intersects(CentralCollisionArea) && !isAttacking && !playerActor.IsDeadOrDying && !IsIndisposed)
            {                
                if (aiNumSeen > 0)
                {
                    aiNewDir = (direction * aiParams.MoveInOldDirectionInfluence) + 
                        aiNewDir * (aiParams.MoveInFlockDirectionInfluence /
                        (float)aiNumSeen);                                        
                }
                else
                {
                    aiNewDir += direction * aiParams.MoveInOldDirectionInfluence;
                }
                //aiNewDir += (playerDir * aiParams.MoveInRandomDirectionInfluence);
                //aiNewDir += (randomDir * aiParams.MoveInRandomDirectionInfluence);
                if(aiNewDir != Vector2.Zero)
                    Vector2.Normalize(ref aiNewDir, out aiNewDir);
                //aiNewDir = ChangeDirection(direction, aiNewDir, aiParams.MaxTurnRadians * elapsedTime);
                direction = aiNewDir;

                
            }
            else
            {
                direction = Vector2.Zero;
                if (!isAttacking && !playerActor.IsDeadOrDying && !IsIndisposed)
                {
                    isAttacking = true;
                }
            }
             
            /*
            if (direction.LengthSquared() > .01f)
            {
                Vector2 moveAmount = direction * moveSpeed * elapsedTime;
                position = position + moveAmount;
                
                //wrap bird to the other side of the screen if needed
                if (position.X < 0.0f)
                {
                    position.X = boundryWidth + position.X;
                }
                else if (position.X > boundryWidth)
                {
                    position.X = position.X - boundryWidth;
                }

                position.Y += direction.Y * moveSpeed * elapsedTime;
                if (position.Y < 0.0f)
                {
                    position.Y = boundryHeight + position.Y;
                }
                else if (position.Y > boundryHeight)
                {
                    position.Y = position.Y - boundryHeight;
                }
                 
            }
             * */
            Move(direction, direction != Vector2.Zero, gameTime);
            UpdateCharacterState();
            UpdateDirectionChanged();


            if (State == CharacterState.Attack && !(attackTimeElapsed <= TimeSpan.Zero))
            {
                return;
            }
            else
            {
                UpdateAnimation(gameTime);
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
            float distance = 0;
            float angle = 0;

            Vector2 playerCentralPosition = playerActor.HitPosition;
            CalcotateAngle(ref distance, ref angle, playerCentralPosition);
            /*
            spriteBatch.DrawString(
                GameplayScreen.debugFont, 
                String.Format("distance: {0}\nattack radius: {1}\nangle: {2}\nisInRadius: {3}\n", distance, attackRadius, angle, distance <= attackRadius),
                new Vector2(0,20),
                Color.White);
             * */
            if (debugVisible)
            {
                spriteBatch.DrawString(
                    GameplayScreen.debugFont,
                    String.Format("isHit: {0}", playerIsHit),
                    new Vector2(0, 20),
                    Color.White);
            }
            base.Draw(spriteBatch, gameTime);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Instantiates all the behaviors that this Bird knows about
        /// </summary>
        public void BuildBehaviors()
        {
            Behaviors humanReactions = new Behaviors();
            humanReactions.Add(new HuntBehavior(this));
            behaviors.Add(ActorType.Human, humanReactions);

            Behaviors alienReactions = new Behaviors();
            alienReactions.Add(new AlignBehavior(this));
            alienReactions.Add(new CohesionBehavior(this));
            alienReactions.Add(new SeparationBehavior(this));
            behaviors.Add(ActorType.Alien, alienReactions);
        }

        /// <summary>
        /// Setup the bird to figure out it's new movement direction
        /// </summary>
        /// <param name="AIparams">flock AI parameters</param>
        public void ResetThink()
        {
            direction = Vector2.Zero;
            aiNewDir = Vector2.Zero;
            aiNumSeen = 0;
            reactionDistance = 0f;
            reactionLocation = Vector2.Zero;
        }

        /// <summary>
        /// Since we're wrapping movement around the screen, two point at extreme 
        /// sides of the screen are actually very close together, this function 
        /// figures out if destLocation is closer the srcLocation if you wrap around
        /// the screen
        /// </summary>
        /// <param name="srcLocation">screen location of src</param>
        /// <param name="destLocation">screen location of dest</param>
        /// <param name="outVector">relative location of dest to src</param>
        private void ClosestLocation(ref Vector2 srcLocation,
            ref Vector2 destLocation, out Vector2 outLocation)
        {
            outLocation = new Vector2();
            float x = destLocation.X;
            float y = destLocation.Y;
            float dX = Math.Abs(destLocation.X - srcLocation.X);
            float dY = Math.Abs(destLocation.Y - srcLocation.Y);
            
            // now see if the distance between birds is closer if going off one
            // side of the map and onto the other.
            if (Math.Abs(boundryWidth - destLocation.X + srcLocation.X) < dX)
            {
                dX = boundryWidth - destLocation.X + srcLocation.X;
                x = destLocation.X - boundryWidth;
            }
            if (Math.Abs(boundryWidth - srcLocation.X + destLocation.X) < dX)
            {
                dX = boundryWidth - srcLocation.X + destLocation.X;
                x = destLocation.X + boundryWidth;
            }

            if (Math.Abs(boundryHeight - destLocation.Y + srcLocation.Y) < dY)
            {
                dY = boundryHeight - destLocation.Y + srcLocation.Y;
                y = destLocation.Y - boundryHeight;
            }
            if (Math.Abs(boundryHeight - srcLocation.Y + destLocation.Y) < dY)
            {
                dY = boundryHeight - srcLocation.Y + destLocation.Y;
                y = destLocation.Y + boundryHeight;
            }
            outLocation.X = x;
            outLocation.Y = y;
        }

        /// <summary>
        /// React to an Animal based on it's type
        /// </summary>
        /// <param name="animal"></param>
        public void ReactTo(Actor actor, ref AIParameters AIparams)
        {
            if (actor != null)
            {
                //setting the the reactionLocation and reactionDistance here is
                //an optimization, many of the possible reactions use the distance
                //and location of theAnimal, so we might as well figure them out
                //only once !
                Vector2 otherLocation = actor.Position;
                ClosestLocation(ref position, ref otherLocation,
                    out reactionLocation);
                reactionDistance = Vector2.Distance(position, reactionLocation);
                
                Behaviors reactions = behaviors[actor.ActorType];
                foreach (Behavior reaction in reactions)
                {
                    if (actor.ActorType == ActorType.Alien)
                    {
                        if (reactionDistance < AIparams.DetectionDistance)
                        {
                            reaction.Update(actor, AIparams);
                            if (reaction.Reacted)
                            {
                                aiNewDir += reaction.Reaction;
                                aiNumSeen++;
                            }
                        }
                    }
                    else if(actor.ActorType == ActorType.Human)
                    {
                        reaction.Update(actor, AIparams);
                        if (reaction.Reacted)
                        {
                            aiNewDir += reaction.Reaction;
                            aiNumSeen++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This function clamps turn rates to no more than maxTurnRadians
        /// </summary>
        /// <param name="oldDir">current movement direction</param>
        /// <param name="newDir">desired movement direction</param>
        /// <param name="maxTurnRadians">max turn in radians</param>
        /// <returns></returns>
        private static Vector2 ChangeDirection(
            Vector2 oldDir, Vector2 newDir, float maxTurnRadians)
        {
            float oldAngle = (float)Math.Atan2(oldDir.Y, oldDir.X);
            float desiredAngle = (float)Math.Atan2(newDir.Y, newDir.X);
            float newAngle = MathHelper.Clamp(desiredAngle, WrapAngle(
                    oldAngle - maxTurnRadians), WrapAngle(oldAngle + maxTurnRadians));
            return new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));
        }
        /// <summary>
        /// clamps the angle in radians between -Pi and Pi.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        #endregion

        public virtual bool ContainsPoint(Vector2 point1, Vector2 point2, float distance)
        {
            return Vector2.Distance(point1, point2) < distance;
        }

        public virtual bool IsPlayerHit()
        {
            bool playerHit = false;

            float distance = 0;
            float angle = 0;
            Vector2 alienHitPosition = HitPosition;
            Vector2 playerHitPosition = playerActor.HitPosition;

            // Calcolate angle between alienHitPosition and PlayerHitPosition
            Vector2 angleVector = playerHitPosition - alienHitPosition;
            angle = MathHelper.ToDegrees((float)Math.Atan2(angleVector.Y, angleVector.X));

            switch (CardDirection)
            {
                case CardinalDirection.North:
                    distance = 30;
                    if (ContainsPoint(alienHitPosition, playerHitPosition, distance))
                    {
                        if (angle <= 0 && angle >= -180)
                        {
                            playerHit = true;
                        }
                    }
                    break;
                case CardinalDirection.South:
                    distance = 30;
                    if (ContainsPoint(alienHitPosition, playerHitPosition, distance))
                    {
                        if (angle >= 0 && angle <= 180)
                        {
                            playerHit = true;
                        }
                    }
                    break;
                case CardinalDirection.West:
                    distance = 45;
                    if (ContainsPoint(alienHitPosition, playerHitPosition, distance))
                    {
                        //if (angle >= 160 && angle <= -160)
                        if(angle >= 90 || angle <= -90)
                        {
                            playerHit = true;
                        }
                    }
                    break;
                case CardinalDirection.East:
                    distance = 45;
                    if (ContainsPoint(alienHitPosition, playerHitPosition, distance))
                    {
                        //if (angle <= 20 && angle >= -20)
                        if(angle >= -90 && angle <= 90)
                        {
                            playerHit = true;
                        }
                    }
                    break;
                case CardinalDirection.NorthWest:
                    distance = 50;
                    if (ContainsPoint(alienHitPosition, playerHitPosition, distance))
                    {
                        //if (angle >= -160 && angle <= -130)
                        if(angle >= -180 && angle <= -90)
                        {
                            playerHit = true;
                        }
                    }
                    break;
                case CardinalDirection.NorthEast:
                    distance = 50;
                    if (ContainsPoint(alienHitPosition, playerHitPosition, distance))
                    {
                        //if (angle >= -50 && angle <= -20)
                        if(angle >= -90 && angle <= 0)
                        {
                            playerHit = true;
                        }
                    }
                    break;
                case CardinalDirection.SouthWest:
                    distance = 50;
                    if (ContainsPoint(alienHitPosition, playerHitPosition, distance))
                    {
                        //if (angle >= 120 && angle <= 180)
                        if(angle >= 90 && angle <= 180)
                        {
                            playerHit = true;
                        }
                    }
                    break;
                case CardinalDirection.SouthEast:
                    distance = 50;
                    if (ContainsPoint(alienHitPosition, playerHitPosition, distance))
                    {
                        //if (angle >= 0 && angle <= 60)
                        if(angle >= 0 && angle <= 90)
                        {
                            playerHit = true;
                        }
                    }
                    break;
                default:
                    playerHit = false;
                    break;
            }

            return playerHit;
        }

        private void CalcotateAngle(ref float distance, ref float angle, Vector2 point)
        {
            Vector2 AlienCentralPosition = HitPosition;

            distance = Vector2.Distance(AlienCentralPosition, point);

            Vector2 angleVector =  point - AlienCentralPosition;

            angle = MathHelper.ToDegrees((float)Math.Atan2(angleVector.Y, angleVector.X));
        }

        protected override float CalcolateLayerDepth()
        {
            if (!IsDeadOrDying)
                return base.CalcolateLayerDepth();
            else
            {
                switch (CardDirection)
                {
                    case CardinalDirection.South:                        
                    case CardinalDirection.SouthWest:                        
                    case CardinalDirection.SouthEast:
                        if (CurrentAnimatingSprite.CurrentRowFrame >= 3)
                        {
                            return 0f;
                        }
                        else
                        {
                            return base.CalcolateLayerDepth();
                        }
                    case CardinalDirection.North:
                        return MathHelper.Clamp(((Position.Y + 48f) / worldSize.Y), 0f, 1f);

                    default:
                        return base.CalcolateLayerDepth();
                }
            }
        }

        public void HandleHitState()
        {
            PlaySound("enemyDead");
            isHit = true;
        }

    }
}
