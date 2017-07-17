using System;

namespace JoyConToPC.Input.Type
{
    public interface IJoyCon : IDisposable
    {
        bool IsConnected { get; }
        bool IsAcquired { get; }
        bool IsPolling { get; }
        JoyConPlayer Player { get; }
        bool IsDisposed { get; }

        event EventHandler<JoyConDataUpdateEventArgs> DataUpdated;

        void Acquire(JoyConPlayer player);
        void Unacquire();

        void StartPolling();
        void StopPolling();

        void SetupLeds(JoyConLed led);

        void SetupLeds(JoyConSingleLed firstLed, JoyConSingleLed secondLed,
            JoyConSingleLed thirdLed, JoyConSingleLed fourthLed);
    }

    public class JoyConDataUpdateEventArgs : EventArgs
    {
        //TODO
    }
}