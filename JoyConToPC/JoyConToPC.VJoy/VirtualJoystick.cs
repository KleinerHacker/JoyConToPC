using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.VJoy.Type;
using JoyConToPC.VJoy.Util;

namespace JoyConToPC.VJoy
{
    public sealed class VirtualJoystick : IDisposable
    {
        public uint DeviceId { get; }
        public bool IsDisposed { get; private set; }

        internal VirtualJoystick(uint deviceId)
        {
            DeviceId = deviceId;
        }

        public void SendData(VirtualJoystickData data)
        {
            var vJoyData = data.ToNative();
            VJoyNatives.SendData(DeviceId, ref vJoyData);
        }

        public void Dispose()
        {
            if (IsDisposed)
                throw new InvalidOperationException("already disposed");

            VJoyNatives.UnacquireVJoy(DeviceId);
            IsDisposed = true;
        }
    }
}
