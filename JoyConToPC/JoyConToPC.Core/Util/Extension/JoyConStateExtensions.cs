using System;
using JoyConToPC.Input.Type;
using JoyConToPC.Input.Util;
using JoyConToPC.VJoy.Type;

namespace JoyConToPC.Core.Util.Extension
{
    internal static class JoyConStateExtensions
    {
        public static VirtualJoystickData ToVirtualJoystickData(this JoyConState joyConState, IJoyCon joyCon)
        {
            var data = new VirtualJoystickData();

            if (joyCon is JoyConPair)
            {
                HandleJoyConPair(joyConState, (JoyConPair) joyCon, ref data);
            }
            else
            {
                HandleJoyCon(joyConState, ref data);
            }

            return data;
        }

        private static void HandleJoyCon(JoyConState joyConState, ref VirtualJoystickData data)
        {
            if (joyConState is JoyConLeftState)
            {
                var state = (JoyConLeftState) joyConState;

                data.ButtonB = state.ButtonDown;
                data.ButtonX = state.ButtonUp;
                data.ButtonA = state.ButtonLeft;
                data.ButtonY = state.ButtonRight;

                data.BackButtonLeft = state.SideLeftButton;
                data.BackButtonRight = state.SideRightButton;
                data.StartButton = state.MinusButton;
                data.StickLeftButton = state.StickButton;

                data.LeftAxisX = JoyConConstants.LeftJoyConXMultiplier * state.StickHorizontal +
                                 JoyConConstants.LeftJoyConOffsetX;
                data.LeftAxisY = JoyConConstants.LeftJoyConYMultiplier * state.StickVertical +
                                 JoyConConstants.LeftJoyConOffsetY;
            }
            else if (joyConState is JoyConRightState)
            {
                var state = (JoyConRightState) joyConState;

                data.ButtonB = state.ButtonX;
                data.ButtonY = state.ButtonY;
                data.ButtonA = state.ButtonA;
                data.ButtonX = state.ButtonB;

                data.BackButtonLeft = state.SideLeftButton;
                data.BackButtonRight = state.SideRightButton;
                data.StartButton = state.PlusButton;
                data.StickLeftButton = state.StickButton;

                data.LeftAxisX = JoyConConstants.LeftJoyConXMultiplier * state.StickHorizontal +
                                 JoyConConstants.LeftJoyConOffsetX;
                data.LeftAxisY = JoyConConstants.LeftJoyConYMultiplier * state.StickVertical +
                                 JoyConConstants.LeftJoyConOffsetY;
            }
            else
                throw new NotImplementedException();
        }

        private static void HandleJoyConPair(JoyConState joyConState, JoyConPair joyCon, ref VirtualJoystickData data)
        {
            if (joyConState is JoyConLeftState)
            {
                var state = (JoyConLeftState) joyConState;

                data.ButtonUp = state.ButtonUp;
                data.ButtonDown = state.ButtonDown;
                data.ButtonLeft = state.ButtonLeft;
                data.ButtonRight = state.ButtonRight;

                data.BackButtonLeft = state.BackButton;
                data.RearBackButtonLeft = state.RearBackButton;
                data.SelectButton = state.MinusButton;
                data.StickLeftButton = state.StickButton;

                data.LeftAxisX = JoyConConstants.LeftJoyConXMultiplier * state.StickHorizontal +
                                 JoyConConstants.LeftJoyConOffsetX;
                data.LeftAxisY = JoyConConstants.LeftJoyConYMultiplier * state.StickVertical +
                                 JoyConConstants.LeftJoyConOffsetY;

                //Merge With Other JoyCon
                if (joyCon?.RightJoyCon.CurrentState != null)
                {
                    HandleJoyConPair(joyCon.RightJoyCon.CurrentState, null, ref data);
                }
            }
            else if (joyConState is JoyConRightState)
            {
                var state = (JoyConRightState) joyConState;

                data.ButtonY = state.ButtonX;
                data.ButtonX = state.ButtonY;
                data.ButtonB = state.ButtonA;
                data.ButtonA = state.ButtonB;

                data.BackButtonRight = state.BackButton;
                data.RearBackButtonRight = state.RearBackButton;
                data.StartButton = state.PlusButton;
                data.StickRightButton = state.StickButton;

                data.RightAxisX = JoyConConstants.RightJoyConXMultiplier * state.StickHorizontal +
                                  JoyConConstants.RightJoyConOffsetX;
                data.RightAxisY = JoyConConstants.RightJoyConYMultiplier * state.StickVertical +
                                  JoyConConstants.RightJoyConOffsetY;

                //Merge With Other JoyCon
                if (joyCon?.LeftJoyCon.CurrentState != null)
                {
                    HandleJoyConPair(joyCon.LeftJoyCon.CurrentState, null, ref data);
                }
            }
            else
                throw new NotImplementedException();
        }
    }
}