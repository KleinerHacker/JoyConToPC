using System;
using System.Threading;
using System.Threading.Tasks;
using HidLibrary;
using JoyConToPC.Input.Util.Extension;

namespace JoyConToPC.Input.Type
{
    public sealed class JoyCon : IJoyCon
    {
        #region Properties

        public string Guid { get; }
        public JoyConType Type { get; }

        public bool IsConnected => _device.IsConnected;
        public bool IsAcquired => _device.IsOpen;
        public JoyConPlayer Player { get; private set; }
        public bool IsPolling => _pollingTask != null;

        public bool IsDisposed { get; private set; }

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

            Guid = device.ReadSerialNumber();
            Type = joyConType.Value;
        }

        public void Acquire(JoyConPlayer player)
        {
            //lock (this)
            {
                if (IsDisposed)
                    throw new InvalidOperationException("Already disposed");
                if (IsAcquired)
                    throw new InvalidOperationException("Already acquired");

                _device.OpenDevice();
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
                if (IsPolling)
                {
                    StopPolling();
                }

                _device.CloseDevice();
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
                _pollingTask = null;
            }
        }

        public void SetupLeds(JoyConLed led)
        {
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
            if (!IsAcquired)
                throw new InvalidOperationException("Is not acquired yet");

            byte light = 0;
            CalculateLight(ref light, firstLed, 1);
            CalculateLight(ref light, secondLed, 2);
            CalculateLight(ref light, thirdLed, 4);
            CalculateLight(ref light, fourthLed, 8);

            _device.Write(1, 0x30, new[] {light});

            //TODO: Rumble
            /*byte[] buf = new byte[0x9];
            buf[1 + 0] = 200;
            buf[1 + 4] = 200;
            buf[1 + 0 + 1] = 0x1;
            buf[1 + 4 + 1] = 0x1;
            _device.Write(0x10, buf);*/
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

            _device.Dispose();
            IsDisposed = true;
        }

        private void Poll()
        {
            //TODO
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