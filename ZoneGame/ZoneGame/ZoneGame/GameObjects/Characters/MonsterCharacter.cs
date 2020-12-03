using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ZoneGame
{
    class MonsterCharacter : StandardCharacter
    {
        #region Fields Character Attack

        public bool IsAttacking
        {
            get { return isAttacking; }
        }
        protected bool isAttacking;

        #endregion

        #region Graphic Data

        private AnimatingSprite attackSprite;

        public AnimatingSprite AttackSprite
        {
            get { return attackSprite; }
            protected set { attackSprite = value; }
        }

        #endregion

        #region Initialization

        public MonsterCharacter(
            AnimatingSprite idleSprite,
            AnimatingSprite walkingSprite,
            AnimatingSprite dyingSprite,
            AnimatingSprite attackSprite,
            Vector2 worldSize)
            : base(idleSprite, walkingSprite, dyingSprite, worldSize)
        {
            this.attackSprite = attackSprite;
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
                if (IsMoving && !isAttacking)
                {
                    AnimateStateWalking();
                }
                else
                {
                    if (!isAttacking)
                        AnimateStateIdle();
                    else
                    {
                        AnimateStateAttack();                        
                    }
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
            }
        }

        #region Public Methods

        /// <summary>
        /// Reset the animations for this character.
        /// </summary>
        public override void ResetAnimation(bool isWalking)
        {
            base.ResetAnimation(isWalking);

            if (attackSprite["Attack" + CardDirection.ToString()] != null)
            {
                attackSprite.PlayAnimation("Attack", CardDirection, true);
            }
        }

        public void AnimateStateAttack()
        {
            if (State != CharacterState.Attack)
            {
                State = CharacterState.Attack;
                CurrentAnimatingSprite = AttackSprite;
                AttackSprite.PlayAnimation("Attack", CardDirection, true);
            }
        }

        public override void DirectionChanged()
        {

            if (State != CharacterState.Attack)
            {
                AttackSprite.PlayAnimation("Attack", CardDirection, true);
            }
            else
            {
                AttackSprite.PlayAnimation("Attack", CardDirection, false);
            }

            base.DirectionChanged();
        }

        #endregion
    }
}
