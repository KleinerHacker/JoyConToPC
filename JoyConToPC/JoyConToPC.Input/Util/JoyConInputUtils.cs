using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Input.Type;

namespace JoyConToPC.Input.Util
{
    internal static class JoyConInputUtils
    {
        public static JoyConState ReadInput(byte[] data, JoyConType type)
        {
            if (data[0] == 0x3F)
            {

            }
            else if (data[0] == 0x21 || data[0] == 0x31)
            {
                switch (type)
                {
                        case JoyConType.Left:
                            return ReadLeftInput(data);
                        case JoyConType.Right:
                            return ReadRightInput(data);
                    default:
                        throw new NotImplementedException();
                }
            }

            return null;
        }

        private static JoyConLeftState ReadLeftInput(byte[] data)
        {
            var state = new JoyConLeftState();

            byte specialState = data[5];
            byte globalState = data[4];

            state.ButtonDown = (specialState & 0x01) != 0;
            state.ButtonUp = (specialState & 0x02) != 0;
            state.ButtonRight = (specialState & 0x04) != 0;
            state.ButtonLeft = (specialState & 0x08) != 0;
            state.SideRightButton = (specialState & 0x10) != 0;
            state.SideLeftButton = (specialState & 0x20) != 0;
            state.BackButton = (specialState & 0x40) != 0;
            state.RearBackButton = (specialState & 0x80) != 0;

            state.MinusButton = (globalState & 0x01) != 0;
            state.StickButton = (globalState & 0x08) != 0;
            state.CaptureButton = (globalState & 0x20) != 0;

            state.RawStickHorizontal = (((data[7] & 0x0F) << 4) | ((data[6] & 0xF0) >> 4));
            state.RawStickVertical = data[8];

            state.BatteryState = (data[7] & 0xF0) >> 4;

            return state;
        }

        private static JoyConRightState ReadRightInput(byte[] data)
        {
            var state = new JoyConRightState();

            byte specialState = data[3];
            byte globalState = data[4];

            state.ButtonY = (specialState & 0x01) != 0;
            state.ButtonX = (specialState & 0x02) != 0;
            state.ButtonB = (specialState & 0x04) != 0;
            state.ButtonA = (specialState & 0x08) != 0;
            state.SideRightButton = (specialState & 0x10) != 0;
            state.SideLeftButton = (specialState & 0x20) != 0;
            state.BackButton = (specialState & 0x40) != 0;
            state.RearBackButton = (specialState & 0x80) != 0;

            state.PlusButton = (globalState & 0x02) != 0;
            state.StickButton = (globalState & 0x04) != 0;
            state.HomeButton = (globalState & 0x10) != 0;

            state.RawStickHorizontal = (((data[10] & 0x0F) << 4) | ((data[9] & 0xF0) >> 4));
            state.RawStickVertical = data[11];

            state.BatteryState = (data[10] & 0xF0) >> 4; 

            return state;
        }
    }
}
