using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.VJoy.Type;
using JoyConToPC.VJoy.Util;
using JoyConToPC.VJoy.Util.Extension;
using log4net;

namespace JoyConToPC.VJoy
{
    public sealed class VirtualJoystick : IDisposable
    {
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(VirtualJoystick));

        static VirtualJoystick()
        {
            if (!VJoyNatives.IsVJoyEnabled())
                throw new InvalidOperationException("Unable to find VJoy Driver");
        }

        #region Properties

        public uint DeviceId { get; }
        public VJoyDeviceProfile Profile { get; }
        public bool IsAquired => VJoyNatives.IsVJoyOpen(DeviceId);
        public UsageState UsageState => VJoyNatives.GetVJoyState(DeviceId).ToUsageState();
        public bool IsDisposed { get; private set; }

        #endregion

        public VirtualJoystick(uint deviceId, VJoyDeviceProfile profile)
        {
            DeviceId = deviceId;
            Profile = profile;

            VJoyConfig.CreateDevice(DeviceId, Profile);
        }

        #region Aquiration

        public bool Aquire()
        {
            if (IsDisposed)
                throw new InvalidOperationException("already disposed");
            if (IsAquired)
                throw new InvalidOperationException("Already aquired");

            Logger.Info("Aquire virtual joystick " + this);

            return VJoyNatives.AcquireVJoy(DeviceId);
        }

        public void Unaquire()
        {
            if (IsDisposed)
                throw new InvalidOperationException("already disposed");
            if (!IsAquired)
                throw new InvalidOperationException("Not aquired yet");

            Logger.Info("Unaquire virtual joystick " + this);

            VJoyNatives.UnacquireVJoy(DeviceId);
        }

        #endregion

        public void SendData(VirtualJoystickData data)
        {
            if (IsDisposed)
                throw new InvalidOperationException("already disposed");
            if (!IsAquired)
                throw new InvalidOperationException("Not aquired yet");

            var vJoyData = data.ToNative();
            VJoyNatives.SendData(DeviceId, ref vJoyData);
        }

        public void Dispose()
        {
            if (IsDisposed)
                throw new InvalidOperationException("already disposed");

            Logger.Debug("Dispose virtual joystick " + this);

            if (IsAquired)
            {
                Unaquire();
            }

            //VJoyConfig.DeleteDevice(DeviceId);

            IsDisposed = true;
        }

        #region Equals / Hashcode

        private bool Equals(VirtualJoystick other)
        {
            return DeviceId == other.DeviceId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is VirtualJoystick && Equals((VirtualJoystick) obj);
        }

        public override int GetHashCode()
        {
            return (int) DeviceId;
        }

        #endregion

        public override string ToString()
        {
            return $"{nameof(DeviceId)}: {DeviceId}, {nameof(Profile)}: {Profile}";
        }
    }

    public enum UsageState
    {
        Free,
        OwnUse,
        OtherUse
    }
}
