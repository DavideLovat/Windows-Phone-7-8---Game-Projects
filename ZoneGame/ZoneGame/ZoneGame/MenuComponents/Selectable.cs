using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public class Selectable: ISelectable
    {
        #region Fields

        public event EventHandler selected;

        #endregion

        #region Properties

        public Selectable()
        {
            IsSelectable = true;
        }

        #endregion

        #region Interfaces Methods

        public virtual bool IsSelectable
        {
            get;
            set;
        }

        public virtual EventHandler Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        public virtual void OnSelected()
        {
            if (Selected != null && IsSelectable)
                Selected(this, EventArgs.Empty);
        }

        #endregion

    }
}
