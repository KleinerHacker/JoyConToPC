using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidLibrary;
using JoyConToPC.Input.Type;

namespace JoyConToPC.Input.Util.Extension
{
    internal static class HidDeviceExtensions
    {
        private static int _globalCounter = 0;

        public static JoyConType? ToJoyConType(this HidDevice device)
        {
            if (device.Attributes.ProductId == JoyConConstants.JoyConLeft)
                return JoyConType.Left;
            else if (device.Attributes.ProductId == JoyConConstants.JoyConRight)
                return JoyConType.Right;

            return null;
        }

        public static string ReadSerialNumber(this HidDevice device)
        {
            byte[] serialNumber;
            var success = device.ReadSerialNumber(out serialNumber);

            if (!success)
                return null;

            if (serialNumber.Length > 32)
            {
                byte[] tmp = new byte[32];
                Array.Copy(serialNumber, 0, tmp, 0, tmp.Length);
                serialNumber = tmp;
            }
            return serialNumber.ToHexString();
        }

        public static bool Write(this HidDevice device, byte command, byte[] cmdData)
        {
            byte[] buf = new byte[cmdData.Length + 1];

            buf[0] = command;
            Array.Copy(cmdData, 0, buf, 1, cmdData.Length);

            return device.Write(buf);
        }

        public static bool Write(this HidDevice device, byte command, byte[] cmdData, byte subCommand,
            byte[] subCmdData)
        {
            byte[] buf = new byte[cmdData.Length + 1 + subCmdData.Length];

            Array.Copy(cmdData, 0, buf, 0, cmdData.Length);
            buf[cmdData.Length] = subCommand;
            Array.Copy(subCmdData, 0, buf, cmdData.Length + 1, subCmdData.Length);

            return device.Write(command, buf);
        }

        public static bool Write(this HidDevice device, byte command, byte subCommand,
            byte[] subCmdData)
        {
            return Write(device, command, new byte[] {(byte)(++_globalCounter & 0xF), 0, 1, 0x40, 0x40, 0, 1, 0x40, 0x40}, subCommand, subCmdData);
        }
    }
}