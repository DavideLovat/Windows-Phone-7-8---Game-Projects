using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class SeparationBehavior : Behavior
    {
        #region Initialization

        public SeparationBehavior(Actor actor)
            :base(actor)
        {
        }

        #endregion

        #region Update

        /// <summary>
        /// separationBehavior.Update infuences the owning animal to move away from
        /// the otherAnimal is it’s too close, in this case if it’s inside 
        /// AIParameters.separationDistance.
        /// </summary>
        /// <param name="otherAnimal">the Animal to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public override void Update(Actor otherActor, AIParameters aiParams)
        {
            base.ResetReaction();

            Vector2 pushDirection = Vector2.Zero;
            float weight = aiParams.PerMemberWeight;

            if (Actor.ReactionDistance > 0.0f &&
                Actor.ReactionDistance <= aiParams.SeparationDistance)
            {
                //The otherAnimal is too close so we figure out a pushDirection 
                //vector in the opposite direction of the otherAnimal and then weight
                //that reaction based on how close it is vs. our separationDistance
                if (otherActor.Direction != Vector2.Zero)
                {

                    pushDirection = Actor.Position - Actor.ReactionLocation;
                    Vector2.Normalize(ref pushDirection, out pushDirection);

                    //push away
                    weight *= (1 -
                        (float)Actor.ReactionDistance / aiParams.SeparationDistance);

                    pushDirection *= weight;

                    reacted = true;
                    reaction += pushDirection;
                }
            }
        }
        #endregion
    }
}
