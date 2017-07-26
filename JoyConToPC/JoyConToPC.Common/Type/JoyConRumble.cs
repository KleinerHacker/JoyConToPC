using System;
using System.Collections.Generic;

namespace JoyConToPC.Common.Type
{
    [Serializable]
    public struct JoyConRumble
    {
        public byte Frequency { get; }
        public byte Intensity { get; }

        public JoyConRumble(byte frequency, byte intensity)
        {
            Frequency = frequency;
            Intensity = intensity;
        }
    }

    [Serializable]
    public sealed class JoyConRumbleInfo
    {
        public IDictionary<JoyConRumbleIndex, JoyConRumble> RumbleDict { get; } =
            new Dictionary<JoyConRumbleIndex, JoyConRumble>();

        public JoyConRumbleInfo()
        {
        }

        public JoyConRumbleInfo(JoyConRumble rumble) : this()
        {
            RumbleDict[JoyConRumbleIndex.First] = rumble;
            RumbleDict[JoyConRumbleIndex.Second] = rumble;
        }

        public JoyConRumbleInfo(byte frequency, byte intensity) : this(new JoyConRumble(frequency, intensity))
        {
        }
    }

    public enum JoyConRumbleIndex
    {
        First,
        Second
    }
}