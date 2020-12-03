using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneGame
{
    public class Padding
    {
        #region Properties

        public uint Top
        { get; set; }

        public uint Right
        { get; set; }
        
        public uint Bottom
        { get; set; }
        
        public uint Left
        { get; set; }

        public static Padding Zero
        {
            get { return new Padding(0,0,0,0); }
        }
        
        #endregion

        public Padding(uint top_right_bottom_left)
            :this(top_right_bottom_left, top_right_bottom_left)
        { }

        public Padding(uint top_bottom, uint right_left)
            :this(top_bottom, right_left, top_bottom)
        { }

        public Padding(uint top, uint right_left, uint bottom)
            :this(top, right_left, bottom, right_left)
        { }

        public Padding(uint top, uint right, uint left, uint bottom)
        {
            AssignValues(top, right, left, bottom);
        }

        #region Methods

        public void AssignValues(uint top_right_bottom_left) 
        {
            AssignValues(top_right_bottom_left, top_right_bottom_left);
        }

        public void AssignValues(uint top_bottom, uint right_left) 
        {
            AssignValues(top_bottom, right_left, top_bottom);
        }

        public void AssignValues(uint top, uint right_left, uint bottom) 
        {
            AssignValues(top, right_left, bottom, right_left);
        }

        public void AssignValues(uint top, uint right, uint left, uint bottom) 
        {
            Top = top;
            Right = right;
            Left = left;
            Bottom = bottom;
        }

        #endregion
    }
}
