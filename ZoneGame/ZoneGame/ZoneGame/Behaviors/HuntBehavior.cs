using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ZoneGame
{
    public class HuntBehavior : Behavior
    {
        #region Initialization

        public HuntBehavior(Actor actor)
            : base(actor)
        {
        }

        #endregion

        #region Update

        public override void Update(Actor otherActor, AIParameters aiParams)
        {
            base.ResetReaction();

            Vector2 preyDirection = Vector2.Zero;

            float weight = 1f;
            reacted = true;

            preyDirection = -(Actor.Position - otherActor.Position);
            Vector2.Normalize(ref preyDirection, out preyDirection);

            preyDirection *= weight;

            reaction = preyDirection;//(aiParams.PerPreyWeight * preyDirection);
        }

        #endregion
    }
}
