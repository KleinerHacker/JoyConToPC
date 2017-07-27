using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using JoyConToPC.API;

namespace JoyConToPC.Service.Base.WCF
{
    internal static class ServiceFactory
    {
        public static ServiceHost CreateJoyConDriverConnector()
        {
            ServiceHost serviceHost = new ServiceHost(typeof(JoyConDriverConnector), new Uri("http://localhost:19557/JoyConDriver/Connector"));
            serviceHost.Description.Behaviors.Add(new ServiceMetadataBehavior{HttpGetEnabled = true});

            var serviceDebugBehavior = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
            serviceDebugBehavior.HttpHelpPageUrl = new Uri("http://localhost:19557");

            serviceHost.AddServiceEndpoint(typeof(IJoyConDriverConnector), new BasicHttpBinding(), string.Empty);

            return serviceHost;
        }
    }
}
