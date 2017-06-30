using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JoyConToPC.Input.Type;
using JoyConToPC.Input.Util;
using JoyConToPC.Input.Util.Extension;
using SharpDX.DirectInput;

namespace JoyConToPC.Input
{
    public static class JoyConFactory
    {
        private static readonly object Monitor = new object();

        private static IList<JoyCon> GetAvailableRawJoyConList()
        {
            var result = new List<JoyCon>();
            
            var devices = InputCore.GetDevices();
            foreach (DeviceInstance device in devices)
            {
                var joyConType = device.ProductGuid.ToJoyConType();
                if (!joyConType.HasValue)
                    continue;

                var joyCon = new JoyCon(device.InstanceGuid, joyConType.Value);
                try
                {
                    joyCon.Acquire(1, Process.GetCurrentProcess().MainWindowHandle);
                    joyCon.Unacquire();
                }
                catch (Exception)
                {
                    continue;
                }

                result.Add(joyCon);
            }

            return result;
        }

        public static IList<IJoyCon> GetAvailableJoyConList()
        {
            lock (Monitor)
            {
                var joyConRawList = GetAvailableRawJoyConList();

                var joyConList = new List<IJoyCon>();

                //Find all pairs
                while (true)
                {
                    var leftJoyCon = joyConRawList.FirstOrDefault(joyCon => joyCon.Type == JoyConType.Left);
                    var rightJoyCon = joyConRawList.FirstOrDefault(joyCon => joyCon.Type == JoyConType.Right);

                    if (leftJoyCon != null && rightJoyCon != null)
                    {
                        joyConList.Add(new JoyConPair(leftJoyCon, rightJoyCon));
                        joyConRawList.Remove(leftJoyCon);
                        joyConRawList.Remove(rightJoyCon);
                    }
                    else
                    {
                        break;
                    }
                }

                //Find all singles
                joyConList.AddRange(joyConRawList);

                return joyConList;
            }
        }
    }
}
