using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.VJoy.Type;

namespace JoyConToPC.VJoy.Util
{
    internal static class VJoyNatives
    {
        [DllImport("vJoyInterface.dll", EntryPoint = "vJoyEnabled")]
        public static extern bool IsVJoyEnabled();

        [DllImport("vJoyInterface.dll", EntryPoint = "GetVJDStatus")]
        public static extern VJoyState GetVJoyState(uint deviceId);

        [DllImport("vJoyInterface.dll", EntryPoint = "AcquireVJD")]
        public static extern bool AcquireVJoy(uint deviceId);

        [DllImport("vJoyInterface.dll", EntryPoint = "RelinquishVJD")]
        public static extern void UnacquireVJoy(uint deviceId);

        [DllImport("vJoyInterface.dll", EntryPoint = "UpdateVJD")]
        public static extern void SendData(uint deviceId, ref VJoyData value);

        [DllImport("vJoyInterface.dll", EntryPoint = "isVJDOpen")]
        public static extern bool IsVJoyOpen(uint deviceId);
    }

    internal enum VJoyState
    {
        IsOwn = 0,
        IsFree = 1,
        IsBusy = 2,
        IsMissed = 3,
        IsUnknown = 4
    }
}
