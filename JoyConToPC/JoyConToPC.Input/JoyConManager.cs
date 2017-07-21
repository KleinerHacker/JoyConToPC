using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using JoyConToPC.Input.Type;

namespace JoyConToPC.Input
{
    public sealed class JoyConManager : IDisposable
    {
        #region Properties

        public IReadOnlyCollection<IJoyCon> JoyConList
        {
            get
            {
                var list = new List<IJoyCon>();
                list.AddRange(_singleJoyConList);
                list.AddRange(_pairJoyConList);

                return new ReadOnlyCollection<IJoyCon>(list);
            }
        }

        public bool IsDisposed { get; private set; }

        private readonly IList<JoyCon> _rawJoyConList = new List<JoyCon>();
        private readonly IList<JoyCon> _singleJoyConList = new List<JoyCon>();
        private readonly IList<JoyConPair> _pairJoyConList = new List<JoyConPair>();

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
            _rawJoyConList.Clear();
            _singleJoyConList.Clear();
            _pairJoyConList.Clear();

            IsDisposed = true;
        }

        private void OnConnectionTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var availableJoyConList = JoyConFactory.GetJoyConList();

            //Find all new
            foreach (var joyCon in availableJoyConList)
            {
                if (_rawJoyConList.Contains(joyCon))
                    continue;

                _rawJoyConList.Add(joyCon);
                _singleJoyConList.Add(joyCon);

                JoyConUpdated?.Invoke(this, new JoyConUpdateEventArgs(joyCon, JoyConUpdateType.Connected));
            }

            //Find all removed
            foreach (var joyCon in _rawJoyConList)
            {
                if (availableJoyConList.Contains(joyCon))
                    continue;

                _rawJoyConList.Remove(joyCon);
                if (_singleJoyConList.Contains(joyCon))
                {
                    _singleJoyConList.Remove(joyCon);
                }
                else if (_pairJoyConList.Any(jc => jc.LeftJoyCon.Equals(joyCon) || jc.RightJoyCon.Equals(joyCon)))
                {
                    var pair = _pairJoyConList.First(
                        jc => jc.LeftJoyCon.Equals(joyCon) || jc.RightJoyCon.Equals(joyCon));
                    _pairJoyConList.Remove(pair);
                    _singleJoyConList.Add(pair.LeftJoyCon.Equals(joyCon) ? pair.RightJoyCon : pair.LeftJoyCon);
                }

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