using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.VJoy.Util.Extension
{
    internal static class NativeToPublicExtensions
    {
        public static UsageState ToUsageState(this VJoyState state)
        {
            switch (state)
            {
                case VJoyState.IsFree:
                case VJoyState.IsMissed:
                case VJoyState.IsUnknown:
                    return UsageState.Free;
                case VJoyState.IsBusy:
                    return UsageState.OtherUse;
                case VJoyState.IsOwn:
                    return UsageState.OwnUse;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}