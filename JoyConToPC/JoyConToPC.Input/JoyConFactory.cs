using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HidLibrary;
using JoyConToPC.Input.Type;
using JoyConToPC.Input.Util;
using JoyConToPC.Input.Util.Extension;

namespace JoyConToPC.Input
{
    internal static class JoyConFactory
    {
        private static readonly object Monitor = new object();

        public static IList<JoyCon> GetJoyConList()
        {
            var result = new List<JoyCon>();

            var devices = HidDevices.Enumerate(JoyConConstants.Vendor);
            foreach (var device in devices)
            {
                var joyConType = device.ToJoyConType();
                if (!joyConType.HasValue)
                    continue;
                
                var joyCon = new JoyCon(device);
                result.Add(joyCon);
            }

            return result;
        }

        /*public static IList<IJoyCon> GetJoyConList()
        {
            lock (Monitor)
            {
                var joyConRawList = GetRawJoyConList();

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
        }*/
    }
}
