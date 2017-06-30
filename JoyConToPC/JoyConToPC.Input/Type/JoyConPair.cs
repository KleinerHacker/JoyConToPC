using System;

namespace JoyConToPC.Input.Type
{
    public sealed class JoyConPair : IJoyCon
    {
        #region Properties

        internal JoyCon LeftJoyCon { get; }
        internal JoyCon RightJoyCon { get; }

        public bool IsAcquired => LeftJoyCon.IsAcquired || RightJoyCon.IsAcquired;
        public bool IsPolling => LeftJoyCon.IsPolling || RightJoyCon.IsPolling;
        public int PlayerNumber => LeftJoyCon.PlayerNumber;
        public bool IsDisposed => LeftJoyCon.IsDisposed || RightJoyCon.IsDisposed;

        #endregion

        #region Events

        public event EventHandler<JoyConDataUpdateEventArgs> DataUpdated;

        #endregion

        internal JoyConPair(JoyCon leftJoyCon, JoyCon rightJoyCon)
        {
            if (leftJoyCon.Type != JoyConType.Left)
                throw new ArgumentException("Left JoyCon is not marked as LEFT");
            if (rightJoyCon.Type != JoyConType.Right)
                throw new ArgumentException("Right JoyCon is not marked as RIGHT");

            LeftJoyCon = leftJoyCon;
            RightJoyCon = rightJoyCon;

            LeftJoyCon.DataUpdated += (sender, args) => DataUpdated?.Invoke(this, args);
            RightJoyCon.DataUpdated += (sender, args) => DataUpdated?.Invoke(this, args);
        }

        public void Acquire(int number, IntPtr handle)
        {
            LeftJoyCon.Acquire(number, handle);
            RightJoyCon.Acquire(number, handle);
        }

        public void Unacquire()
        {
            LeftJoyCon.Unacquire();
            RightJoyCon.Unacquire();
        }

        public void StartPolling()
        {
            LeftJoyCon.StartPolling();
            RightJoyCon.StartPolling();
        }

        public void StopPolling()
        {
            LeftJoyCon.StopPolling();
            RightJoyCon.StopPolling();
        }

        public void Dispose()
        {
            LeftJoyCon.Dispose();
            RightJoyCon.Dispose();
        }

        #region Equals / Hashcode

        private bool Equals(JoyConPair other)
        {
            return Equals(LeftJoyCon, other.LeftJoyCon) && Equals(RightJoyCon, other.RightJoyCon);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is JoyConPair && Equals((JoyConPair) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((LeftJoyCon != null ? LeftJoyCon.GetHashCode() : 0) * 397) ^ (RightJoyCon != null ? RightJoyCon.GetHashCode() : 0);
            }
        }

        #endregion

        public override string ToString()
        {
            return $"JoyCon Pair ({LeftJoyCon.Guid}|{RightJoyCon.Guid})";
        }
    }
}
