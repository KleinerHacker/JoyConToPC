using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.Input.Util.Extension
{
    internal static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] array)
        {
            StringBuilder sb = new StringBuilder(array.Length * 2);
            foreach (var b in array)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }
}
