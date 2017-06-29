using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using SharpDX.XInput;
using DeviceType = SharpDX.DirectInput.DeviceType;

namespace JoyConToPC.Core
{
    internal static class InputCore
    {
        private static readonly DirectInput Input = new DirectInput();

        public static IList<DeviceInstance> GetDevices()
        {
            return Input.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly);
        }

        public static Joystick Acquire(Guid deviceGuid, IntPtr handle)
        {
            var joystick = new Joystick(Input, deviceGuid);
            foreach (var doi in joystick.GetObjects(DeviceObjectTypeFlags.Axis))
            {
                joystick.GetObjectPropertiesById(doi.ObjectId).Range = new InputRange(-5000, 5000);
            }

            joystick.Properties.AxisMode = DeviceAxisMode.Absolute;
            joystick.Properties.BufferSize = 128;
            joystick.SetCooperativeLevel(handle, (CooperativeLevel.Exclusive | CooperativeLevel.Background));
            joystick.Acquire();

            return joystick;
        }

        public static void Unacquire(Joystick joystick)
        {
            joystick.Unacquire();
            joystick.Dispose();
        }
    }
}
