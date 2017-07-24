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
        private static readonly object Monitor = new object();

        static VirtualJoystickManager()
        {
            if (!VJoyNatives.IsVJoyEnabled())
                throw new InvalidOperationException("Unable to find VJoy");
        }

        public static bool IsVirtualJoystickUsable(uint deviceId)
        {
            lock (Monitor)
            {
                var state = VJoyNatives.GetVJoyState(deviceId);
                return state == VJoyState.IsFree /*|| state == VJoyState.IsMissed || state == VJoyState.IsUnknown*/;
            }
        }

        public static VirtualJoystick GetVirtualJoystick(uint deviceId, VJoyDeviceProfile profile)
        {
            lock (Monitor)
            {
                //VJoyConfig.CreateDevice(deviceId, profile);

                if (!VJoyNatives.AcquireVJoy(deviceId))
                    throw new InvalidOperationException("Unable to aquire VJoy on device id " + deviceId);

                return new VirtualJoystick(deviceId, profile);
            }
        }
    }
}
