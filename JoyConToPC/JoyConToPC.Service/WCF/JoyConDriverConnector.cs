using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.API;
using JoyConToPC.API.Type;

namespace JoyConToPC.Service.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class JoyConDriverConnector : IJoyConDriverConnector
    {
        public CJoyCon[] GetJoyConList()
        {
            return new CJoyCon[0];
        }
    }
}
