using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.VJoy.Util;

namespace JoyConToPC.VJoy
{
    public static class VirtualJoystickManager
    {
        static VirtualJoystickManager()
        {
            if (!VJoyNatives.IsVJoyEnabled())
                throw new InvalidOperationException("Unable to find VJoy");
        }

        public static bool IsVirtualJoystickUsable(uint deviceId)
        {
            var state = VJoyNatives.GetVJoyState(deviceId);
            return state == VJoyState.IsFree;
        }

        public static VirtualJoystick GetVirtualJoystick(uint deviceId)
        {
            if (!VJoyNatives.AcquireVJoy(deviceId))
                throw new InvalidOperationException("Unable to aquire VJoy on device id " + deviceId);

            return new VirtualJoystick(deviceId);
        }
    }
}
