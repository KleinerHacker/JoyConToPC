using System;
using System.Threading;
using System.Threading.Tasks;
using JoyConToPC.Input.Util;
using SharpDX.DirectInput;

namespace JoyConToPC.Input.Type
{
    public sealed class JoyCon : IJoyCon
    {
        #region Properties

        public Guid Guid { get; }
        public JoyConType Type { get; }

        public bool IsAcquired { get; private set; }
        public int PlayerNumber { get; private set; }
        public bool IsPolling => _pollingTask != null;

        public bool IsDisposed { get; private set; }

        #endregion

        #region Events

        public event EventHandler<JoyConDataUpdateEventArgs> DataUpdated;

        #endregion

        private readonly Joystick _joystick;
        private CancellationTokenSource _cts;
        private Task _pollingTask;

        public JoyCon(Guid guid, JoyConType type)
        {
            Guid = guid;
            Type = type;

            _joystick = InputCore.CreateJoystick(guid);
        }

        public void Acquire(int number, IntPtr handle)
        {
            //lock (this)
            {
                if (IsDisposed)
                    throw new InvalidOperationException("Already disposed");
                if (IsAcquired)
                    throw new InvalidOperationException("Already acquired");

                _joystick.SetCooperativeLevel(handle, (CooperativeLevel.Exclusive | CooperativeLevel.Background));
                _joystick.Acquire();
                PlayerNumber = number;

                IsAcquired = true;
            }
        }

        public void Unacquire()
        {
            //lock (this)
            {
                if (IsDisposed)
                    throw new InvalidOperationException("Already disposed");
                if (!IsAcquired)
                    throw new InvalidOperationException("Is not acquired yet");
                if (IsPolling)
                {
                    StopPolling();
                }

                _joystick.Unacquire();

                IsAcquired = false;
            }
        }

        public void StartPolling()
        {
            //lock (this)
            {
                if (IsDisposed)
                    throw new InvalidOperationException("Already disposed");
                if (!IsAcquired)
                    throw new InvalidOperationException("Is not acquired yet");
                if (IsPolling)
                    throw new InvalidOperationException("Already polling");

                _cts = new CancellationTokenSource();
                _pollingTask = Task.Run(() => PollingTask(), _cts.Token);
            }
        }

        public void StopPolling()
        {
            //lock (this)
            {
                if (IsDisposed)
                    throw new InvalidOperationException("Already disposed");
                if (!IsAcquired)
                    throw new InvalidOperationException("Is not acquired yet");
                if (!IsPolling)
                    throw new InvalidOperationException("Is not polling yet");

                _cts.Cancel();
                _cts = null;
                _pollingTask = null;
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
                throw new InvalidOperationException("Already disposed");

            if (IsPolling)
            {
                StopPolling();
            }
            if (IsAcquired)
            {
                Unacquire();
            }

            _joystick.Dispose();
            IsDisposed = true;
        }

        private void Poll()
        {
            _joystick.Poll();
            var states = _joystick.GetBufferedData();
            foreach (var state in states)
            {
                DataUpdated?.Invoke(this, new JoyConDataUpdateEventArgs(state));
            }
        }

        private void PollingTask()
        {
            while (!_cts.IsCancellationRequested)
            {
                Poll();
                Thread.Sleep(10);
            }
        }

        #region Equals / Hashcode

        private bool Equals(JoyCon other)
        {
            return Guid.Equals(other.Guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is JoyCon && Equals((JoyCon) obj);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        #endregion

        public override string ToString()
        {
            return $"JoyCon {Type} ({Guid})";
        }
    }

    public enum JoyConType
    {
        Left,
        Right
    }

    public class JoyConDataUpdateEventArgs : EventArgs
    {
        public JoystickUpdate State { get; }

        public JoyConDataUpdateEventArgs(JoystickUpdate state)
        {
            State = state;
        }
    }
}
