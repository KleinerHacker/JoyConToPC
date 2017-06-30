using System;

namespace JoyConToPC.Input.Type
{
    public interface IJoyCon : IDisposable
    {
        bool IsAcquired { get; }
        bool IsPolling { get; }
        int PlayerNumber { get; }
        bool IsDisposed { get; }

        event EventHandler<JoyConDataUpdateEventArgs> DataUpdated;

        void Acquire(int number, IntPtr handle);
        void StartPolling();
        void StopPolling();
        void Unacquire();
    }
}