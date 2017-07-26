using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.Service.WCF
{
    internal static class ServiceFactory
    {
        public static ServiceHost CreateJoyConDriverConnector()
        {
            ServiceHost serviceHost = new ServiceHost(typeof(JoyConDriverConnector));



            return serviceHost;
        }
    }
}
