using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace ZoneGame
{
    class AlignBehavior : Behavior
    {
        #region Initialization

        public AlignBehavior(Actor actor)
            : base(actor)
        {
        }

        #endregion

        #region Update

        /// <summary>
        /// AlignBehavior.Update infuences the owning animal to move in same the 
        /// direction as the otherAnimal that it sees.
        /// </summary>
        /// <param name="otherAnimal">the Animal to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public override void Update(Actor otherActor, AIParameters aiParams)
        {
            base.ResetReaction();

            if (otherActor != null && otherActor.Direction != Vector2.Zero)
            {
                    reacted = true;
                    reaction = otherActor.Direction * aiParams.PerMemberWeight;
            }
        }
        #endregion
    }
}
