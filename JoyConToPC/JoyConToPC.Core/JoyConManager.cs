using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using JoyConToPC.Input;
using JoyConToPC.Input.Type;

namespace JoyConToPC.Core
{
    internal sealed class JoyConManager
    {
        #region Properties

        public IList<IJoyCon> JoyConList { get; } = new List<IJoyCon>();

        #endregion

        #region Events

        public event EventHandler<JoyConUpdateEventArgs> JoyConUpdated;

        #endregion

        private readonly Timer _connectionTimer = new Timer {Enabled = true, Interval = 1000};

        public JoyConManager()
        {
            _connectionTimer.Elapsed += OnConnectionTimerElapsed;
        }

        private void OnConnectionTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var availableJoyConList = JoyConFactory.GetAvailableJoyConList();

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
