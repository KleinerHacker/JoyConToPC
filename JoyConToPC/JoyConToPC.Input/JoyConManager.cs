using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using JoyConToPC.Common.Type;
using JoyConToPC.Input.Type;
using log4net;
using Timer = System.Timers.Timer;

namespace JoyConToPC.Input
{
    public sealed class JoyConManager : IDisposable
    {
        private static readonly object ListMonitor = new object();
        private static readonly object SplittingMonitor = new object();
        private static readonly object PairingMonitor = new object();

        private static ILog Logger { get; } = LogManager.GetLogger(typeof(JoyConManager));

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

        private readonly Task _connectionTask;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        //A joycon it is ready to pair
        private JoyCon _readyToPairJoyCon;
        //A joycon it is ready to split
        private JoyCon _readyToSplitJoyCon;

        public JoyConManager()
        {
            _connectionTask = Task.Run(() =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    OnConnectionTimerElapsed();
                    Thread.Sleep(5000);
                }
            }, _cts.Token);
        }

        public void Dispose()
        {
            if (IsDisposed)
                throw new InvalidOperationException("already disposed");

            _cts.Cancel();
            _connectionTask.Wait();
            _connectionTask.Dispose();

            lock (ListMonitor)
            {
                foreach (var joyCon in JoyConList)
                {
                    JoyConUpdated?.Invoke(this, new JoyConUpdateEventArgs(joyCon, JoyConUpdateType.Disconnected));
                }
                _rawJoyConList.Clear();
                _singleJoyConList.Clear();
                _pairJoyConList.Clear();
            }

            IsDisposed = true;
        }

        private void OnConnectionTimerElapsed()
        {
            var availableJoyConList = JoyConFactory.GetJoyConList();

            lock (ListMonitor)
            {
                //Find all new
                foreach (var joyCon in availableJoyConList)
                {
                    if (_rawJoyConList.Contains(joyCon))
                        continue;

                    joyCon.SetupLeds(JoyConLed.FlashAll);

                    _rawJoyConList.Add(joyCon);
                    _singleJoyConList.Add(joyCon);

                    joyCon.Pairing += OnJoyConPairing;
                    joyCon.Splitting += OnJoyConSplitting;

                    Logger.Info("Find new JoyCon: " + joyCon);
                    JoyConUpdated?.Invoke(this, new JoyConUpdateEventArgs(joyCon, JoyConUpdateType.Connected));
                }

                //Find all removed
                foreach (var joyCon in new List<JoyCon>(_rawJoyConList))
                {
                    if (availableJoyConList.Contains(joyCon))
                        continue;

                    joyCon.SetupLeds(JoyConLed.FlashAll);

                    joyCon.Pairing -= OnJoyConPairing;
                    joyCon.Splitting -= OnJoyConSplitting;
                    _rawJoyConList.Remove(joyCon);
                    if (_singleJoyConList.Contains(joyCon))
                    {
                        _singleJoyConList.Remove(joyCon);
                        Logger.Info("Find removed JoyCon: " + joyCon);
                        JoyConUpdated?.Invoke(this, new JoyConUpdateEventArgs(joyCon, JoyConUpdateType.Disconnected));
                    }
                    else if (_pairJoyConList.Any(jc => jc.LeftJoyCon.Equals(joyCon) || jc.RightJoyCon.Equals(joyCon)))
                    {
                        var pair = _pairJoyConList.First(
                            jc => jc.LeftJoyCon.Equals(joyCon) || jc.RightJoyCon.Equals(joyCon));
                        _pairJoyConList.Remove(pair);
                        Logger.Info("Find new JoyCon: " + joyCon);
                        JoyConUpdated?.Invoke(this, new JoyConUpdateEventArgs(joyCon, JoyConUpdateType.Disconnected));
                        // Search stayed joycon and unpair, add to single joy con list and make connection message
                        var stayedJoyCon = pair.LeftJoyCon.Equals(joyCon) ? pair.RightJoyCon : pair.LeftJoyCon;
                        stayedJoyCon.UnPair(); //TODO: In Dispose of JoyConPair?
                        _singleJoyConList.Add(stayedJoyCon);
                        JoyConUpdated?.Invoke(this,
                            new JoyConUpdateEventArgs(stayedJoyCon, JoyConUpdateType.Connected));
                    }
                }
            }
        }

        private void OnJoyConSplitting(object sender, JoyConSplittingEventArgs args)
        {
            lock (SplittingMonitor)
            {
                if (args.SplittingType == SplittingType.ReadyToSplit)
                {
                    if (_readyToSplitJoyCon == null)
                    {
                        Logger.Debug("JoyCon Ready to Split: " + args.SourceJoyCon);
                        _readyToSplitJoyCon = args.SourceJoyCon;
                    }
                    else
                    {
                        if (_readyToSplitJoyCon.Equals(args.SourceJoyCon) || 
                            _readyToSplitJoyCon.Type == args.SourceJoyCon.Type)
                            return;

                        var joyConPair = _pairJoyConList.FirstOrDefault(
                            joyCon => (joyCon.LeftJoyCon.Equals(_readyToSplitJoyCon) ||
                                       joyCon.LeftJoyCon.Equals(args.SourceJoyCon)) &&
                                      (joyCon.RightJoyCon.Equals(_readyToSplitJoyCon) ||
                                       joyCon.LeftJoyCon.Equals(args.SourceJoyCon)));
                        if (joyConPair == null)
                            return;

                        Logger.Debug("Start splitting of: " + args.SourceJoyCon + " and " + _readyToSplitJoyCon);

                        lock (ListMonitor)
                        {
                            //Remove joycon pair
                            _pairJoyConList.Remove(joyConPair);
                            JoyConUpdated?.Invoke(this, new JoyConUpdateEventArgs(joyConPair, JoyConUpdateType.Disconnected));
                            //Add joycon singles
                            joyConPair.LeftJoyCon.UnPair();
                            _singleJoyConList.Add(joyConPair.LeftJoyCon);
                            JoyConUpdated?.Invoke(this,
                                new JoyConUpdateEventArgs(joyConPair.LeftJoyCon, JoyConUpdateType.Connected));
                            _singleJoyConList.Add(joyConPair.RightJoyCon);
                            JoyConUpdated?.Invoke(this,
                                new JoyConUpdateEventArgs(joyConPair.RightJoyCon, JoyConUpdateType.Connected));
                        }

                        _readyToSplitJoyCon = null;
                    }
                }
                else if (args.SplittingType == SplittingType.CancelSplitting)
                {
                    Logger.Debug("JoyCon Splitting canceled: " + args.SourceJoyCon);
                    _readyToSplitJoyCon = null;
                }
                else
                    throw new NotImplementedException();
            }
        }

        private void OnJoyConPairing(object sender, JoyConPairingEventArgs args)
        {
            lock (PairingMonitor)
            {
                if (args.PairingType == PairingType.ReadyToPair)
                {
                    if (_readyToPairJoyCon == null)
                    {
                        Logger.Debug("JoyCon Ready to Pair: " + args.SourceJoyCon);
                        _readyToPairJoyCon = args.SourceJoyCon;
                    }
                    else
                    {
                        if (_readyToPairJoyCon.Equals(args.SourceJoyCon) ||
                            _readyToPairJoyCon.Type == args.SourceJoyCon.Type)
                            return; //Detect same or same type (left / right) of joycon

                        Logger.Debug("Start pairing of: " + args.SourceJoyCon + " and " + _readyToPairJoyCon);

                        //Run pairing
                        var leftJoyCon = _readyToPairJoyCon.Type == JoyConType.Left
                            ? _readyToPairJoyCon
                            : args.SourceJoyCon;
                        var rightJoyCon = _readyToPairJoyCon.Type == JoyConType.Right
                            ? _readyToPairJoyCon
                            : args.SourceJoyCon;
                        lock (ListMonitor)
                        {
                            //Remove singles
                            _singleJoyConList.Remove(leftJoyCon);
                            JoyConUpdated?.Invoke(this,
                                new JoyConUpdateEventArgs(leftJoyCon, JoyConUpdateType.Disconnected));
                            _singleJoyConList.Remove(rightJoyCon);
                            JoyConUpdated?.Invoke(this,
                                new JoyConUpdateEventArgs(rightJoyCon, JoyConUpdateType.Disconnected));
                            //Add pair as new
                            var pairedJoyCon = new JoyConPair(leftJoyCon, rightJoyCon);
                            leftJoyCon.PairWith(rightJoyCon); //TODO: In JoyConPair?
                            _pairJoyConList.Add(pairedJoyCon);
                            JoyConUpdated?.Invoke(this,
                                new JoyConUpdateEventArgs(pairedJoyCon, JoyConUpdateType.Connected));
                        }

                        _readyToPairJoyCon = null;
                    }
                }
                else if (args.PairingType == PairingType.CancelPairing)
                {
                    Logger.Debug("JoyCon Pairing canceled: " + args.SourceJoyCon);
                    _readyToPairJoyCon = null;
                }
                else
                    throw new NotImplementedException();
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