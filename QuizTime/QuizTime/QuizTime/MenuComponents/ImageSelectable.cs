using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace QuizTime
{
    public class ImageSelectable:Image, ISelectable
    {
        #region Fields

        Selectable selectable = new Selectable();

        #endregion

        #region Initialization

        public ImageSelectable(Texture2D imageContents)
            :base(imageContents)
        { }

        public ImageSelectable(int index, Texture2D imageContents)
            : base(index, imageContents)
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
            if (!IsActive)
            {
                return;
            }
            selectable.OnSelected();
        }

        #endregion
    }
}
