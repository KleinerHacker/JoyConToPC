using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.VJoy.Type
{
    public struct VirtualJoystickData
    {
        public bool ButtonX { get; set; }
        public bool ButtonY { get; set; }
        public bool ButtonA { get; set; }
        public bool ButtonB { get; set; }

        public bool BackButtonLeft { get; set; }
        public bool BackButtonRight { get; set; }
        public bool RearBackButtonLeft { get; set; }
        public bool RearBackButtonRight { get; set; }

        public bool StickLeftButton { get; set; }
        public bool StickRightButton { get; set; }

        public bool ButtonUp { get; set; }
        public bool ButtonDown { get; set; }
        public bool ButtonLeft { get; set; }
        public bool ButtonRight { get; set; }

        public bool StartButton { get; set; }
        public bool SelectButton { get; set; }
        public bool MainButton { get; set; }

        public int LeftAxisX { get; set; }
        public int LeftAxisY { get; set; }

        public int RightAxisX { get; set; }
        public int RightAxisY { get; set; }
    }

    internal static class VirtualJoystickDataExtensions
    {
        public static VJoyData ToNative(this VirtualJoystickData data)
        {
            var nativeData = new VJoyData {buttons = 0};

            if (data.ButtonA)
                nativeData.buttons |= 0x00000001;
            if (data.ButtonB)
                nativeData.buttons |= 0x00000002;
            if (data.ButtonX)
                nativeData.buttons |= 0x00000004;
            if (data.ButtonY)
                nativeData.buttons |= 0x00000008;

            nativeData.hats = 0x00000004;
            if (data.ButtonUp)
                nativeData.hats = 0x00000000;
            if (data.ButtonDown)
                nativeData.hats = 0x00000002;
            if (data.ButtonLeft)
                nativeData.hats = 0x00000003;
            if (data.ButtonRight)
                nativeData.hats = 0x00000001;

            if (data.BackButtonLeft)
                nativeData.buttons |= 0x00000010;
            if (data.BackButtonRight)
                nativeData.buttons |= 0x00000020;

            if (data.StickLeftButton)
                nativeData.buttons |= 0x00000100;
            if (data.StickRightButton)
                nativeData.buttons |= 0x00000200;

            if (data.StartButton)
                nativeData.buttons |= 0x00000080;
            if (data.SelectButton)
                nativeData.buttons |= 0x00000040;

            nativeData.axisX = data.LeftAxisX;
            nativeData.axisY = data.LeftAxisY;
            nativeData.axisXRot = data.RightAxisX;
            nativeData.axisYRot = data.RightAxisY;

            nativeData.axisZ = 16000;
            if (data.RearBackButtonLeft)
                nativeData.axisZ = 0;
            if (data.RearBackButtonRight)
                nativeData.axisZ = 32000;

            return nativeData;
        }
    }
}
