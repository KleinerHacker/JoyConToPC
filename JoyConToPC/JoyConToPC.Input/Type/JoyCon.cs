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
        public bool IsPolling => _pollingTask != null;

        public bool IsDisposed { get; private set; }

        public JoyConState CurrentState { get; private set; }

        #endregion

        #region Events

        public event EventHandler<JoyConDataUpdateEventArgs> DataUpdated;

        #endregion

        private readonly HidDevice _device;
        private CancellationTokenSource _cts;
        private Task _pollingTask;

        public JoyCon(HidDevice device)
        {
            var joyConType = device.ToJoyConType();
            if (joyConType == null)
                throw new ArgumentException("HID Device is not a JoyCon");

            _device = device;
            _device.MonitorDeviceEvents = false;

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

                _device.OpenDevice(DeviceMode.NonOverlapped, DeviceMode.NonOverlapped, ShareMode.ShareRead | ShareMode.ShareWrite);
                Player = player;

                SetupLeds(player.ToJoyConLed());
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

                if (IsPolling)
                {
                    StopPolling();
                }

                _device.CloseDevice();

                SetupLeds(JoyConLed.FlashAll);
            }
        }

        #endregion

        #region Polling

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

                Logger.Info("Start Polling JoyCon " + Guid);

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

                Logger.Info("Stop Polling JoyCon " + Guid);

                _cts.Cancel();
                _pollingTask = null;
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

        public void Dispose()
        {
            if (IsDisposed)
                throw new InvalidOperationException("Already disposed");

            Logger.Info($"Dispose JoyCon {Guid}");

            if (IsPolling)
            {
                StopPolling();
            }
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
            _device.ReadReport(); //Wait for input
            _device.Write(new byte[] { 0x01, 0x00 });
            var deviceData = _device.Read();
            
            if (deviceData.Status != HidDeviceData.ReadStatus.Success)
                return;

            var joyConState = JoyConInputUtils.ReadInput(deviceData.Data, Type);
            if (joyConState != null && !joyConState.Equals(CurrentState))
            {
                CurrentState = joyConState;
                DataUpdated?.Invoke(this, new JoyConDataUpdateEventArgs(this, joyConState));
            }
        }

        private void PollingTask()
        {
            while (!_cts.IsCancellationRequested)
            {
                Poll();
                Thread.Sleep(10);
            }

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
}