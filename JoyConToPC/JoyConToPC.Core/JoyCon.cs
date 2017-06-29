using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using SharpDX.XInput;

namespace JoyConToPC.Core
{
    public sealed class JoyCon
    {
        #region Properties

        public Guid Guid { get; }
        public JoyConType Type { get; }

        public bool IsAcquired => _joystick != null;
        public int PlayerNumber { get; private set; }

        #endregion

        private Joystick _joystick;

        public JoyCon(Guid guid, JoyConType type)
        {
            Guid = guid;
            Type = type;
        }

        public void Acquire(int number, IntPtr handle)
        {
            if (IsAcquired)
                throw new InvalidOperationException("Already acquired");

            _joystick = InputCore.Acquire(Guid, handle);
            PlayerNumber = number;
        }

        public void Unacquire()
        {
            if (!IsAcquired)
                throw new InvalidOperationException("Is not acquired yet");

            InputCore.Unacquire(_joystick);
            _joystick = null;
        }

        public void Poll()
        {
            _joystick.Poll();
            var datas = _joystick.GetBufferedData();
            foreach (var state in datas)
            {
                Console.WriteLine(state);
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

    
}
