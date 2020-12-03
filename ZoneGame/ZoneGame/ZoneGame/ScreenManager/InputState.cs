using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace ZoneGame
{
    public class InputState
    {
        #region Fields

        public const int MaxInputs = 4;

        public readonly GamePadState[] CurrentGamePadStates;
        public readonly GamePadState[] LastGamePadStates;

        public readonly bool[] GamePadWasConnected;

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        #endregion

        #region Public Methods

        public InputState()
        {
            CurrentGamePadStates = new GamePadState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
        }
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastGamePadStates[i] = CurrentGamePadStates[i];
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }

            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
        }

        protected bool CheckButton(Buttons button, PlayerIndex? controllingPlayer)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                PlayerIndex playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                return (CheckButton(button, PlayerIndex.One) ||
                    CheckButton(button, PlayerIndex.Two) ||
                    CheckButton(button, PlayerIndex.Three) ||
                    CheckButton(button, PlayerIndex.Four));
            }
        }

        public bool IsNewButtonPress(Buttons button)
        {
            return (CheckButton(button, PlayerIndex.One) ||
                    CheckButton(button, PlayerIndex.Two) ||
                    CheckButton(button, PlayerIndex.Three) ||
                    CheckButton(button, PlayerIndex.Four));
        }

        public bool IsMenuSelect()
        {
            return IsNewButtonPress(Buttons.Start);
        }

        public bool IsMenuCancel()
        {
            return IsNewButtonPress(Buttons.Back);
        }

        public bool IsPauseGame()
        {
            return IsNewButtonPress(Buttons.Back) ||
                   IsNewButtonPress(Buttons.Start);
        }

        #endregion
    }
}
