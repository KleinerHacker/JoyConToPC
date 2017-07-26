using System;

namespace JoyConToPC.Common.Type
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