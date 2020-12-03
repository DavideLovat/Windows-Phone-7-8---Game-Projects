using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizTime
{
    public interface ISelectable
    {
        //event EventHandler selected;
        bool IsSelectable { get; set; }
        EventHandler Selected { get; set; }
        void OnSelected();
    }
}
