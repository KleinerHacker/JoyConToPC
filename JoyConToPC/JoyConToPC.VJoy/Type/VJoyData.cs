using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.VJoy.Type
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct VJoyData
    {
        public byte device;
        public int throttle;
        public int rudder;
        public int aileron;
        public int axisX;
        public int axisY;
        public int axisZ;
        public int axisXRot;
        public int axisYRot;
        public int axisZRot;
        public int slider;
        public int dial;
        public int wheel;
        public int axisVX;
        public int axisVY;
        public int axisVZ;
        public int axisVBRX;
        public int axisVBRY;
        public int axisVBRZ;
        public int buttons;
        public uint hats;
        public uint hatsEx1;
        public uint hatsEx2;
        public uint hatsEx3;

        public int buttonsEx1;
        public int buttonsEx2;
        public int buttonsEx3;
    }
}
