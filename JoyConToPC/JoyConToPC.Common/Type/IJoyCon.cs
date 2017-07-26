using System;

namespace JoyConToPC.Common.Type
{
    public interface IJoyCon : IDisposable
    {
        bool IsConnected { get; }
        bool IsAcquired { get; }
        JoyConPlayer Player { get; }
        bool IsDisposed { get; }

        event EventHandler<JoyConDataUpdateEventArgs> DataUpdated;

        void Acquire(JoyConPlayer player);
        void Unacquire();

        void SetupLeds(JoyConLed led);
        void SetupLeds(JoyConSingleLed firstLed, JoyConSingleLed secondLed,
            JoyConSingleLed thirdLed, JoyConSingleLed fourthLed);

        void Rumble(JoyConRumble rumble);
        void Rumble(JoyConRumbleInfo rumbleInfo);
    }

    public class JoyConDataUpdateEventArgs : EventArgs
    {
        public IJoyCon JoyConSource { get; }
        public JoyConState JoyConState { get; }

        public JoyConDataUpdateEventArgs(IJoyCon joyConSource, JoyConState joyConState)
        {
            JoyConSource = joyConSource;
            JoyConState = joyConState;
        }
    }
}