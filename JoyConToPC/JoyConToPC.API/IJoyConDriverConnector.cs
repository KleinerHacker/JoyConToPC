using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.API.Type;
using JoyConToPC.Common.Type;

namespace JoyConToPC.API
{
    [ServiceContract]
    public interface IJoyConDriverConnector
    {
        [OperationContract]
        CJoyCon[] GetJoyConList();
    }
}
