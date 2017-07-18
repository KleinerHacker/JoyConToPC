using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Input.Util;

namespace JoyConToPC.Input.Type
{
    [Serializable]
    public abstract class JoyConState
    {
        public bool SideLeftButton { get; internal set; }
        public bool SideRightButton { get; internal set; }

        public bool BackButton { get; internal set; }
        public bool RearBackButton { get; internal set; }

        public bool StickButton { get; internal set; }

        public int RawStickHorizontal { get; internal set; }
        public int RawStickVertical { get; internal set; }
        public int StickHorizontal => RawStickHorizontal - 128;
        public int StickVertical => RawStickVertical - 128;
            
        public int BatteryState { get; internal set; }

        #region Equals / Hashcode

        protected bool Equals(JoyConState other)
        {
            return SideLeftButton == other.SideLeftButton && SideRightButton == other.SideRightButton && BackButton == other.BackButton && RearBackButton == other.RearBackButton && StickButton == other.StickButton && RawStickHorizontal == other.RawStickHorizontal && RawStickVertical == other.RawStickVertical;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JoyConState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SideLeftButton.GetHashCode();
                hashCode = (hashCode * 397) ^ SideRightButton.GetHashCode();
                hashCode = (hashCode * 397) ^ BackButton.GetHashCode();
                hashCode = (hashCode * 397) ^ RearBackButton.GetHashCode();
                hashCode = (hashCode * 397) ^ StickButton.GetHashCode();
                hashCode = (hashCode * 397) ^ RawStickHorizontal;
                hashCode = (hashCode * 397) ^ RawStickVertical;
                hashCode = (hashCode * 397) ^ BatteryState;
                return hashCode;
            }
        }

        #endregion

        public override string ToString()
        {
            return $"{nameof(SideLeftButton)}: {SideLeftButton}, {nameof(SideRightButton)}: {SideRightButton}, {nameof(BackButton)}: {BackButton}, {nameof(RearBackButton)}: {RearBackButton}, {nameof(StickButton)}: {StickButton}, {nameof(RawStickHorizontal)}: {RawStickHorizontal}, {nameof(RawStickVertical)}: {RawStickVertical}, {nameof(BatteryState)}: {BatteryState}";
        }
    }

    [Serializable]
    public sealed class JoyConLeftState : JoyConState
    {
        public bool ButtonDown { get; internal set; }
        public bool ButtonUp { get; internal set; }
        public bool ButtonRight { get; internal set; }
        public bool ButtonLeft { get; internal set; }

        public bool MinusButton { get; internal set; }
        public bool CaptureButton { get; internal set; }

        #region Equals / Hashcode

        private bool Equals(JoyConLeftState other)
        {
            return base.Equals(other) && ButtonDown == other.ButtonDown && ButtonUp == other.ButtonUp && ButtonRight == other.ButtonRight && ButtonLeft == other.ButtonLeft && MinusButton == other.MinusButton && CaptureButton == other.CaptureButton;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is JoyConLeftState && Equals((JoyConLeftState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ ButtonDown.GetHashCode();
                hashCode = (hashCode * 397) ^ ButtonUp.GetHashCode();
                hashCode = (hashCode * 397) ^ ButtonRight.GetHashCode();
                hashCode = (hashCode * 397) ^ ButtonLeft.GetHashCode();
                hashCode = (hashCode * 397) ^ MinusButton.GetHashCode();
                hashCode = (hashCode * 397) ^ CaptureButton.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(ButtonDown)}: {ButtonDown}, {nameof(ButtonUp)}: {ButtonUp}, {nameof(ButtonRight)}: {ButtonRight}, {nameof(ButtonLeft)}: {ButtonLeft}, {nameof(MinusButton)}: {MinusButton}, {nameof(CaptureButton)}: {CaptureButton}";
        }
    }

    [Serializable]
    public sealed class JoyConRightState : JoyConState
    {
        public bool ButtonY { get; internal set; }
        public bool ButtonX { get; internal set; }
        public bool ButtonB { get; internal set; }
        public bool ButtonA { get; internal set; }

        public bool PlusButton { get; internal set; }
        public bool HomeButton { get; internal set; }

        #region Equals / Hashcode

        private bool Equals(JoyConRightState other)
        {
            return base.Equals(other) && ButtonY == other.ButtonY && ButtonX == other.ButtonX && ButtonB == other.ButtonB && ButtonA == other.ButtonA && PlusButton == other.PlusButton && HomeButton == other.HomeButton;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is JoyConRightState && Equals((JoyConRightState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ ButtonY.GetHashCode();
                hashCode = (hashCode * 397) ^ ButtonX.GetHashCode();
                hashCode = (hashCode * 397) ^ ButtonB.GetHashCode();
                hashCode = (hashCode * 397) ^ ButtonA.GetHashCode();
                hashCode = (hashCode * 397) ^ PlusButton.GetHashCode();
                hashCode = (hashCode * 397) ^ HomeButton.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(ButtonY)}: {ButtonY}, {nameof(ButtonX)}: {ButtonX}, {nameof(ButtonB)}: {ButtonB}, {nameof(ButtonA)}: {ButtonA}, {nameof(PlusButton)}: {PlusButton}, {nameof(HomeButton)}: {HomeButton}";
        }
    }
}
