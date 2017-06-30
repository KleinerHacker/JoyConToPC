using System;
using System.Collections.Generic;
using SharpDX.DirectInput;

namespace JoyConToPC.Input.Util
{
    internal static class InputCore
    {
        private static readonly DirectInput Input = new DirectInput();

        public static IList<DeviceInstance> GetDevices()
        {
            return Input.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);
        }

        public static Joystick CreateJoystick(Guid deviceGuid)
        {
            var joystick = new Joystick(Input, deviceGuid);
            foreach (DeviceObjectInstance doi in joystick.GetObjects(DeviceObjectTypeFlags.Axis))
            {
                joystick.GetObjectPropertiesById(doi.ObjectId).Range = new InputRange(-100, 100);
            }

            joystick.Properties.AxisMode = DeviceAxisMode.Absolute;
            joystick.Properties.BufferSize = 128;

            return joystick;
        }
    }
}