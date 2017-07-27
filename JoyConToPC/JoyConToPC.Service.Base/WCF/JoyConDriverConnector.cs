using System.ServiceModel;
using JoyConToPC.API;
using JoyConToPC.API.Type;

namespace JoyConToPC.Service.Base.WCF
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
