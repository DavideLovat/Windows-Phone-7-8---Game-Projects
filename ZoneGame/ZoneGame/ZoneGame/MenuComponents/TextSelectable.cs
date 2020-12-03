using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public class TextSelectable : Text, ISelectable
    {
        #region Fields

        protected Selectable selectable = new Selectable();

        #endregion

        #region Initialization

        public TextSelectable(string textContents, SpriteFont font)
            : base(textContents, font)
        { }

        public TextSelectable(int index, string textContents, SpriteFont font)
            : base(index, textContents, font)
        { }

        #endregion        

        #region Interfaces Methods

        public bool IsSelectable
        {
            get { return selectable.IsSelectable; }
            set { selectable.IsSelectable = value;}
        }

        public EventHandler Selected
        {
            get { return selectable.Selected; }
            set { selectable.Selected = value; }
        }

        public virtual void OnSelected()
        {
                selectable.OnSelected();
        }

        #endregion
    }
}
