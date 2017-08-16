using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using JoyConToPC.API;
using JoyConToPC.Common.Util.Constant;
using log4net;

namespace JoyConToPC.Service.Base.WCF
{
    internal static class ServiceFactory
    {
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(ServiceFactory));

        

        public static ServiceHost CreateJoyConDriverConnector()
        {
            // Create the ServiceHost.
            var host = new ServiceHost(new JoyConDriverConnectorService(), new Uri(NetworkConstants.JoyConDriverConnectorEndpoint));
            // Enable metadata publishing.
            var smb = new ServiceMetadataBehavior
            {
                HttpGetEnabled = true,
                MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 }
            };
            host.Description.Behaviors.Add(smb);


            var binding = new WSHttpBinding(SecurityMode.Message);
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            host.AddServiceEndpoint(typeof(IJoyConDriverConnector), binding, NetworkConstants.JoyConDriverConnectorEndpoint);

            Logger.Info("JoyCon Driver Control Service added: " + NetworkConstants.JoyConDriverConnectorEndpoint);

            return host;
        }
    }
}
