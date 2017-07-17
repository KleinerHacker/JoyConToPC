using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.Input.Type
{
    public enum JoyConPlayer : uint
    {
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4
    }

    public static class JoyConPlayerExtensions
    {
        internal static JoyConLed ToJoyConLed(this JoyConPlayer? player)
        {
            if (player == null)
                return JoyConLed.FlashAll;

            switch (player)
            {
                case JoyConPlayer.First:
                    return JoyConLed.First;
                case JoyConPlayer.Second:
                    return JoyConLed.Second;
                case JoyConPlayer.Third:
                    return JoyConLed.Third;
                case JoyConPlayer.Fourth:
                    return JoyConLed.Fourth;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static JoyConLed ToJoyConLed(this JoyConPlayer player)
        {
            return ToJoyConLed((JoyConPlayer?) player);
        }

        public static JoyConPlayer? Next(this JoyConPlayer player)
        {
            switch (player)
            {
                case JoyConPlayer.First:
                    return JoyConPlayer.Second;
                case JoyConPlayer.Second:
                    return JoyConPlayer.Third;
                case JoyConPlayer.Third:
                    return JoyConPlayer.Fourth;
                case JoyConPlayer.Fourth:
                    return null;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}