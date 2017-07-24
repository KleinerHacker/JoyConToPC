using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HidLibrary;
using JoyConToPC.Input.Util;
using JoyConToPC.Input.Util.Extension;
using log4net;

namespace JoyConToPC.Input.Type
{
    public sealed class JoyCon : IJoyCon
    {
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(JoyCon));

        #region Properties

        public string Guid { get; }
        public JoyConType Type { get; }

        public bool IsConnected => _device.IsConnected;
        public bool IsAcquired => _device.IsOpen;
        public JoyConPlayer Player { get; private set; }
        public bool IsPaired => PairedJoyCon != null;
        public JoyCon PairedJoyCon { get; private set; }

        public bool IsDisposed { get; private set; }

        public JoyConState CurrentState { get; private set; }

        #endregion

        #region Events

        public event EventHandler<JoyConDataUpdateEventArgs> DataUpdated;
        internal event EventHandler<JoyConPairingEventArgs> Pairing;
        internal event EventHandler<JoyConSplittingEventArgs> Splitting;

        #endregion

        private readonly HidDevice _device;
        private CancellationTokenSource _cts;

        private Task _pollingTask;

        //Current state of rear bacl button needed for pairing action
        private bool _currentInPairingState;

        //Current state of splitting
        private bool _currentInSplittingState;

        public JoyCon(HidDevice device)
        {
            var joyConType = device.ToJoyConType();
            if (joyConType == null)
                throw new ArgumentException("HID Device is not a JoyCon");

            _device = device;

            Guid = device.ReadSerialNumber();
            Type = joyConType.Value;
        }

        #region Acquiration

        public void Acquire(JoyConPlayer player)
        {
            //lock (this)
            {
                if (IsDisposed)
                    throw new InvalidOperationException("Already disposed");
                if (IsAcquired)
                    throw new InvalidOperationException("Already acquired");

                Logger.Info("Acquire JoyCon " + Guid);

                _device.OpenDevice();
                Player = player;

                SetupLeds(player.ToJoyConLed());

                _cts = new CancellationTokenSource();
                _pollingTask = Task.Run(() => PollingTask(), _cts.Token);
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

                Logger.Info("Unacquire JoyCon " + Guid);

                _cts.Cancel();
                _pollingTask.Wait();
                _pollingTask.Dispose();
                _pollingTask = null;

                SetupLeds(JoyConLed.FlashAll);
                _device.CloseDevice();
            }
        }

        #endregion

        #region LED

        public void SetupLeds(JoyConLed led)
        {
            Logger.Info($"Set LED {led} to JoyCon {Guid}");

            switch (led)
            {
                case JoyConLed.First:
                    SetupLeds(JoyConSingleLed.On, JoyConSingleLed.Off, JoyConSingleLed.Off, JoyConSingleLed.Off);
                    break;
                case JoyConLed.Second:
                    SetupLeds(JoyConSingleLed.Off, JoyConSingleLed.On, JoyConSingleLed.Off, JoyConSingleLed.Off);
                    break;
                case JoyConLed.Third:
                    SetupLeds(JoyConSingleLed.Off, JoyConSingleLed.Off, JoyConSingleLed.On, JoyConSingleLed.Off);
                    break;
                case JoyConLed.Fourth:
                    SetupLeds(JoyConSingleLed.Off, JoyConSingleLed.Off, JoyConSingleLed.Off, JoyConSingleLed.On);
                    break;
                case JoyConLed.FlashAll:
                    SetupLeds(JoyConSingleLed.Flash, JoyConSingleLed.Flash, JoyConSingleLed.Flash,
                        JoyConSingleLed.Flash);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void SetupLeds(JoyConSingleLed firstLed, JoyConSingleLed secondLed, JoyConSingleLed thirdLed,
            JoyConSingleLed fourthLed)
        {
            if (IsDisposed)
                throw new InvalidOperationException("Already disposed");

            Logger.Info($"Set LEDs {firstLed}, {secondLed}, {thirdLed}, {fourthLed} to JoyCon {Guid}");

            byte light = 0;
            CalculateLight(ref light, firstLed, 1);
            CalculateLight(ref light, secondLed, 2);
            CalculateLight(ref light, thirdLed, 4);
            CalculateLight(ref light, fourthLed, 8);

            bool autoClose = !IsAcquired;
            _device.Write(1, 0x30, new[] {light});
            if (autoClose)
            {
                _device.CloseDevice();
            }
        }

        #endregion

        #region Rumbles

        public void Rumble(JoyConRumble rumble)
        {
            Rumble(new JoyConRumbleInfo(rumble));
        }

        public void Rumble(JoyConRumbleInfo rumbleInfo)
        {
            Logger.Info($"Rumble JoyCon {Guid}");

            //TODO: Rumble not work
            byte[] buf = new byte[0x9];

            if (rumbleInfo.RumbleDict.ContainsKey(JoyConRumbleIndex.First))
            {
                var rumble = rumbleInfo.RumbleDict[JoyConRumbleIndex.First];
                buf[1 + 0] = rumble.Frequency;
                buf[1 + 0 + rumble.Intensity] = 0x1;
            }

            if (rumbleInfo.RumbleDict.ContainsKey(JoyConRumbleIndex.Second))
            {
                var rumble = rumbleInfo.RumbleDict[JoyConRumbleIndex.Second];
                buf[1 + 4] = rumble.Frequency;
                buf[1 + 4 + rumble.Intensity] = 0x1;
            }

            _device.Write(0x10, buf);
        }

        #endregion

        #region Pairing

        /// <summary>
        /// Pair this joycon with an other joycon and around
        /// </summary>
        /// <param name="joyCon"></param>
        internal void PairWith(JoyCon joyCon)
        {
            if (IsPaired)
                throw new InvalidOperationException("Already paired");

            PairedJoyCon = joyCon;
            joyCon.PairedJoyCon = this;
        }

        /// <summary>
        /// Unpair this joycon and around
        /// </summary>
        internal void UnPair()
        {
            if (!IsPaired)
                throw new InvalidOperationException("Not paired yet");

            PairedJoyCon.PairedJoyCon = null;
            PairedJoyCon = null;
        }

        #endregion

        public void Dispose()
        {
            if (IsDisposed)
                throw new InvalidOperationException("Already disposed");

            Logger.Info($"Dispose JoyCon {Guid}");

            if (IsAcquired)
            {
                Unacquire();
            }

            _device.Dispose();
            IsDisposed = true;
        }

        #region Privates

        private void Poll()
        {
            var report = _device.ReadReport();
            if (report == null)
            {
                Logger.Debug("JoyCon Canceled " + this);
                return;
            }

            _device.Write(new byte[] {0x01, 0x00});
            var deviceData = _device.Read();

            if (deviceData.Status != HidDeviceData.ReadStatus.Success)
                return;

            var joyConState = JoyConInputUtils.ReadInput(deviceData.Data, Type);
            if (joyConState != null && !joyConState.Equals(CurrentState))
            {
                CurrentState = joyConState;
                Task.Run(() => DataUpdated?.Invoke(this, new JoyConDataUpdateEventArgs(this, joyConState)));

                if (IsPaired)
                {
                    //Splitting state changed?
                    if (_currentInSplittingState != (joyConState.SideLeftButton || joyConState.SideRightButton))
                    {
                        _currentInSplittingState = joyConState.SideLeftButton || joyConState.SideRightButton;
                        //Invoke splitting event async
                        Task.Run(() => Splitting?.Invoke(this, new JoyConSplittingEventArgs(
                            this, _currentInSplittingState ? SplittingType.ReadyToSplit : SplittingType.CancelSplitting
                        )));
                    }
                }
                else
                {
                    //Pairing state changed?
                    if (_currentInPairingState != joyConState.RearBackButton)
                    {
                        _currentInPairingState = joyConState.RearBackButton;
                        //Invoke ready to pair or paring cancel event async
                        Task.Run(() => Pairing?.Invoke(this, new JoyConPairingEventArgs(
                            this, _currentInPairingState ? PairingType.ReadyToPair : PairingType.CancelPairing
                        )));
                    }
                }
            }
        }

        private void PollingTask()
        {
            Logger.Debug("THREAD: Start Polling");

            while (!_cts.IsCancellationRequested)
            {
                Poll();
                Thread.Sleep(10);
            }

            Logger.Debug("THREAD: Finish Polling");

            _cts.Dispose();
            _cts = null;
        }

        private void CalculateLight(ref byte light, JoyConSingleLed led, byte number)
        {
            var flashNumber = (byte) (number * 0x10);

            switch (led)
            {
                case JoyConSingleLed.On:
                    light |= number;
                    if ((light & flashNumber) != 0)
                        light ^= flashNumber;
                    break;
                case JoyConSingleLed.Off:
                    if ((light & number) != 0)
                        light ^= number;
                    if ((light & flashNumber) != 0)
                        light ^= flashNumber;
                    break;
                case JoyConSingleLed.Flash:
                    if ((light & number) != 0)
                        light ^= number;
                    light |= flashNumber;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Equals / Hashcode

        private bool Equals(JoyCon other)
        {
            return Equals(Guid, other.Guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is JoyCon && Equals((JoyCon) obj);
        }

        public override int GetHashCode()
        {
            return (Guid != null ? Guid.GetHashCode() : 0);
        }

        #endregion

        public override string ToString()
        {
            return $"JoyCon {Type} ({Guid})";
        }
    }

    internal class JoyConSplittingEventArgs : EventArgs
    {
        public JoyCon SourceJoyCon { get; }
        public SplittingType SplittingType { get; }

        public JoyConSplittingEventArgs(JoyCon sourceJoyCon, SplittingType splittingType)
        {
            SourceJoyCon = sourceJoyCon;
            SplittingType = splittingType;
        }
    }

    internal enum SplittingType
    {
        ReadyToSplit,
        CancelSplitting
    }

    internal class JoyConPairingEventArgs : EventArgs
    {
        public JoyCon SourceJoyCon { get; }
        public PairingType PairingType { get; }

        public JoyConPairingEventArgs(JoyCon sourceJoyCon, PairingType pairingType)
        {
            SourceJoyCon = sourceJoyCon;
            PairingType = pairingType;
        }
    }

    internal enum PairingType
    {
        ReadyToPair,
        CancelPairing
    }
}