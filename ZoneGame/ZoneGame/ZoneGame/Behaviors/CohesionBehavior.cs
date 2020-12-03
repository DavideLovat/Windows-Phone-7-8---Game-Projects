using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class CohesionBehavior : Behavior
    {
        #region Initialization

        public CohesionBehavior(Actor actor)
            : base(actor)
        {
        }

        #endregion

        #region Update

        /// <summary>
        /// CohesionBehavior.Update infuences the owning animal to move towards the
        /// otherAnimal that it sees as long as it isn’t too close, in this case 
        /// that means inside the separationDist in the passed in AIParameters.
        /// </summary>
        /// <param name="otherAnimal">the Animal to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public override void Update(Actor otherActor, AIParameters aiParams)
        {
            base.ResetReaction();

            Vector2 pullDirection = Vector2.Zero;
            float weight = aiParams.PerMemberWeight;

            //if the otherAnimal is too close we dont' want to fly any
            //closer to it
            if (Actor.ReactionDistance > 0.0f
                && Actor.ReactionDistance > aiParams.SeparationDistance)
            {
                if (otherActor.Direction != Vector2.Zero)
                {
                    //We want to make the animal move closer the the otherAnimal so we 
                    //create a pullDirection vector pointing to the otherAnimal bird and 
                    //weigh it based on how close the otherAnimal is relative to the 
                    //AIParameters.separationDistance.
                    pullDirection = -(Actor.Position - Actor.ReactionLocation);
                    Vector2.Normalize(ref pullDirection, out pullDirection);
                    
                    weight *= (float)Math.Pow((double)
                        (Actor.ReactionDistance - aiParams.SeparationDistance) /
                            (aiParams.DetectionDistance - aiParams.SeparationDistance), 2);
                    
                    pullDirection *= weight;

                    reacted = true;
                    reaction = pullDirection;
                }
                else
                {
                    reacted = false;
                    reaction = Vector2.Zero;
                }
            }
        }
        #endregion
    }
}
