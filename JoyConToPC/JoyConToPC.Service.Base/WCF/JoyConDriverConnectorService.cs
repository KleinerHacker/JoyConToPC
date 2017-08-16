using System.Linq;
using System.ServiceModel;
using JoyConToPC.API;
using JoyConToPC.API.Type;
using JoyConToPC.Core;
using JoyConToPC.Input.Type;

namespace JoyConToPC.Service.Base.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class JoyConDriverConnectorService : IJoyConDriverConnector
    {
        public CJoyCon[] GetJoyConList()
        {
            return JoyConDriver.Instance.JoyConList
                .Select(con => new CJoyCon((con as JoyCon)?.Type.ToString() ?? "Pair", "ABC"))
                .ToArray();
        }
    }
}
