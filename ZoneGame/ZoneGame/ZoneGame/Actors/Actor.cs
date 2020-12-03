using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public enum ActorType
    {
        // no type
        Generic,        
        
        Human,

        Alien,
    }

    public class Actor : InGameComponent
    {
        #region Fields

        protected float moveSpeed;
       /*
        // texture
        protected Texture2D texture;

        protected Vector2 textureCenter;
        */
        /// <summary>
        /// All the behavior that this animal has
        /// </summary>
        protected Dictionary<ActorType, Behaviors> behaviors;

        /// <summary>
        /// The actor type
        /// </summary>
        public ActorType ActorType
        {
            get
            {
                return actorType;
            }
        }
        protected ActorType actorType = ActorType.Generic;

        /// <summary>
        /// Reaction distance
        /// </summary>
        public float ReactionDistance
        {
            get
            {
                return reactionDistance;
            }
        }
        protected float reactionDistance;

        /// <summary>
        /// Reaction location
        /// </summary>
        public Vector2 ReactionLocation
        {
            get
            {
                return reactionLocation;
            }
        }
        protected Vector2 reactionLocation;

        public Vector2 WorldSize
        {
            get
            {
                return worldSize;
            }

            set
            {
                worldSize = value;
            }
        }
        protected Vector2 worldSize;

        /// <summary>
        /// Direction the animal is moving in
        /// </summary>
        public Vector2 Direction
        {
            get
            {
                return direction;
            }
        }
        protected Vector2 direction;

        #endregion

        #region Initialization
        /// <summary>
        /// Sets the boundries the animal can move in the texture used in Draw
        /// </summary>
        public Actor(Vector2 worldSize)
        {            
            this.worldSize = worldSize;

            moveSpeed = 0.0f;

            behaviors = new Dictionary<ActorType, Behaviors>();
        }

        #endregion
    }
}
