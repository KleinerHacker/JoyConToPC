using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.Core.Util.Extension
{
    internal static class GuidExtensions
    {
        private static readonly Guid LeftJoyConGuid = Guid.Parse("2006057e-0000-0000-0000-504944564944");
        private static readonly Guid RightJoyConGuid = Guid.Parse("2007057e-0000-0000-0000-504944564944");

        public static JoyConType? ToJoyConType(this Guid guid)
        {
            if (guid.Equals(LeftJoyConGuid))
                return JoyConType.Left;
            else if (guid.Equals(RightJoyConGuid))
                return JoyConType.Right;

            return null;
        }
    }
}