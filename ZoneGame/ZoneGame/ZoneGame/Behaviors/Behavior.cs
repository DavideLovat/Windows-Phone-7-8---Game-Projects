using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public abstract class Behavior
    {
        #region Fields
        /// <summary>
        /// Keep track of the animal that this behavior belongs to.
        /// </summary>
        public Actor Actor
        {
            get { return actor; }
            set { actor = value; }
        }
        private Actor actor;

        /// <summary>
        /// Store the behavior reaction here.
        /// </summary>
        public Vector2 Reaction
        {
            get { return reaction; }
        }
        protected Vector2 reaction;

        /// <summary>
        /// Store if the behavior has reaction results here.
        /// </summary>
        public bool Reacted
        {
            get { return reacted; }
        }
        protected bool reacted;

        #endregion

        #region Initialization
       
        protected Behavior(Actor actor)
        {
            this.actor = actor;
        }
        
        #endregion

        #region Update
        /// <summary>
        /// Abstract function that the subclass must impliment. Figure out the 
        /// Behavior reaction here.
        /// </summary>
        /// <param name="otherAnimal">the Animal to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public abstract void Update(Actor otherAnimal, AIParameters aiParams);

        /// <summary>
        /// Reset the behavior reactions from the last Update
        /// </summary>
        protected void ResetReaction()
        {
            reacted = false;
            reaction = Vector2.Zero;
        }
        #endregion
    }
}
