using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.API.Type
{
    [Serializable]
    [DataContract]
    public class CJoyCon
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string SerialNumber { get; set; }

        public CJoyCon()
        {
        }

        public CJoyCon(string name, string serialNumber)
        {
            Name = name;
            SerialNumber = serialNumber;
        }

        #region Equals / Hashcode

        protected bool Equals(CJoyCon other)
        {
            return string.Equals(SerialNumber, other.SerialNumber);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CJoyCon) obj);
        }

        public override int GetHashCode()
        {
            return (SerialNumber != null ? SerialNumber.GetHashCode() : 0);
        }

        #endregion

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(SerialNumber)}: {SerialNumber}";
        }
    }
}
