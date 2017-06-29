using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Core.Util.Extension;
using SharpDX.DirectInput;

namespace JoyConToPC.Core
{
    public static class JoyConFactory
    {
        public static IList<JoyCon> GetAvailableJoyConList()
        {
            var result = new List<JoyCon>();

            var devices = InputCore.GetDevices();
            foreach (var device in devices)
            {
                var joyConType = device.ProductGuid.ToJoyConType();
                if (!joyConType.HasValue)
                    continue;

                var joyCon = new JoyCon(device.InstanceGuid, joyConType.Value);
                result.Add(joyCon);
            }

            return result;
        }
    }
}
