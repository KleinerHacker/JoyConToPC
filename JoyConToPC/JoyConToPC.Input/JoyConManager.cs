using System;
using System.Collections.Generic;
using System.Timers;
using JoyConToPC.Input.Type;

namespace JoyConToPC.Input
{
    public sealed class JoyConManager : IDisposable
    {
        #region Properties

        public IList<IJoyCon> JoyConList { get; } = new List<IJoyCon>();
        public bool IsDisposed { get; private set; }

        #endregion

        #region Events

        public event EventHandler<JoyConUpdateEventArgs> JoyConUpdated;

        #endregion

        private readonly Timer _connectionTimer = new Timer {Enabled = true, Interval = 1000};

        public JoyConManager()
        {
            _connectionTimer.Elapsed += OnConnectionTimerElapsed;
        }

        public void Dispose()
        {
            if (IsDisposed)
                throw new InvalidOperationException("already disposed");

            _connectionTimer.Enabled = false;

            foreach (var joyCon in JoyConList)
            {
                JoyConUpdated?.Invoke(this, new JoyConUpdateEventArgs(joyCon, JoyConUpdateType.Disconnected));
            }
            JoyConList.Clear();

            IsDisposed = true;
        }

        private void OnConnectionTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var availableJoyConList = JoyConFactory.GetJoyConList();

            //Find all new
            foreach (var joyCon in availableJoyConList)
            {
                if (JoyConList.Contains(joyCon))
                    continue;

                JoyConList.Add(joyCon);
                
                JoyConUpdated?.Invoke(this, new JoyConUpdateEventArgs(joyCon, JoyConUpdateType.Connected));
            }

            //Find all removed
            foreach (var joyCon in JoyConList)
            {
                if (availableJoyConList.Contains(joyCon))
                    continue;
                
                JoyConUpdated?.Invoke(this, new JoyConUpdateEventArgs(joyCon, JoyConUpdateType.Disconnected));
            }
        }
    }

    public class JoyConUpdateEventArgs : EventArgs
    {
        public IJoyCon JoyCon { get; }
        public JoyConUpdateType Type { get; }

        public JoyConUpdateEventArgs(IJoyCon joyCon, JoyConUpdateType type)
        {
            JoyCon = joyCon;
            Type = type;
        }
    }

    public enum JoyConUpdateType
    {
        Connected,
        Disconnected
    }
}
