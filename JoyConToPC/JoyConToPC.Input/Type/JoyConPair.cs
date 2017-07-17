using System;

namespace JoyConToPC.Input.Type
{
    public sealed class JoyConPair : IJoyCon
    {
        #region Properties

        internal JoyCon LeftJoyCon { get; }
        internal JoyCon RightJoyCon { get; }

        public bool IsConnected => LeftJoyCon.IsConnected && RightJoyCon.IsConnected;
        public bool IsAcquired => LeftJoyCon.IsAcquired || RightJoyCon.IsAcquired;
        public bool IsPolling => LeftJoyCon.IsPolling || RightJoyCon.IsPolling;
        public JoyConPlayer Player => LeftJoyCon.Player;
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

            LeftJoyCon.DataUpdated += (sender, args) => DataUpdated?.Invoke(this, new JoyConDataUpdateEventArgs(this, args.JoyConState));
            RightJoyCon.DataUpdated += (sender, args) => DataUpdated?.Invoke(this, new JoyConDataUpdateEventArgs(this, args.JoyConState));
        }

        #region Acquiration

        public void Acquire(JoyConPlayer player)
        {
            LeftJoyCon.Acquire(player);
            RightJoyCon.Acquire(player);
        }

        public void Unacquire()
        {
            LeftJoyCon.Unacquire();
            RightJoyCon.Unacquire();
        }

        #endregion

        #region Polling

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

        #endregion

        #region LED

        public void SetupLeds(JoyConLed led)
        {
            LeftJoyCon.SetupLeds(led);
            RightJoyCon.SetupLeds(led);
        }

        public void SetupLeds(JoyConSingleLed firstLed, JoyConSingleLed secondLed, JoyConSingleLed thirdLed,
            JoyConSingleLed fourthLed)
        {
            LeftJoyCon.SetupLeds(firstLed, secondLed, thirdLed, fourthLed);
            RightJoyCon.SetupLeds(firstLed, secondLed, thirdLed, fourthLed);
        }

        #endregion

        #region Rumble

        public void Rumble(JoyConRumble rumble)
        {
            LeftJoyCon.Rumble(rumble);
            RightJoyCon.Rumble(rumble);
        }

        public void Rumble(JoyConRumbleInfo rumbleInfo)
        {
            LeftJoyCon.Rumble(rumbleInfo);
            RightJoyCon.Rumble(rumbleInfo);
        }

        public void Rumble(JoyConType type, JoyConRumble rumble)
        {
            Rumble(type, new JoyConRumbleInfo(rumble));
        }

        public void Rumble(JoyConType type, JoyConRumbleInfo rumbleInfo)
        {
            switch (type)
            {
                case JoyConType.Left:
                    LeftJoyCon.Rumble(rumbleInfo);
                    break;
                case JoyConType.Right:
                    RightJoyCon.Rumble(rumbleInfo);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

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
                return ((LeftJoyCon != null ? LeftJoyCon.GetHashCode() : 0) * 397) ^
                       (RightJoyCon != null ? RightJoyCon.GetHashCode() : 0);
            }
        }

        #endregion

        public override string ToString()
        {
            return $"JoyCon Pair ({LeftJoyCon.Guid}|{RightJoyCon.Guid})";
        }
    }
}