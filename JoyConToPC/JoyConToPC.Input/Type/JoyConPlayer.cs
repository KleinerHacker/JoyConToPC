using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.Input.Type
{
    public enum JoyConPlayer
    {
        First,
        Second,
        Third,
        Fourth
    }

    internal static class JoyConPlayerExtensions
    {
        public static JoyConLed ToJoyConLed(this JoyConPlayer? player)
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

        public static JoyConLed ToJoyConLed(this JoyConPlayer player)
        {
            return ToJoyConLed((JoyConPlayer?) player);
        }
    }
}