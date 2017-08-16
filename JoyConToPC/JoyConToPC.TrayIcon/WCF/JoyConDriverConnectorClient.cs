using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.API;
using JoyConToPC.API.Type;
using JoyConToPC.Common.Util.Constant;

namespace JoyConToPC.TrayIcon.WCF
{
    internal class JoyConDriverConnectorClient : IJoyConDriverConnector
    {
        public static JoyConDriverConnectorClient Instance { get; } = new JoyConDriverConnectorClient();

        private JoyConDriverConnectorClient()
        {
        }

        public CJoyCon[] GetJoyConList()
        {
            return GetData(client => client.GetJoyConList());
        }

        private T GetData<T>(Func<IJoyConDriverConnector, T> func)
        {
            T value = default(T);
            Execute(client => value = func(client));
            return value;
        }

        private void Execute(Action<IJoyConDriverConnector> executor)
        {
            using (var scf = new ChannelFactory<IJoyConDriverConnector>(CreateBinding(), NetworkConstants.JoyConDriverConnectorEndpoint))
            {
                var s = scf.CreateChannel();
                executor(s);
            }
        }

        private WSHttpBinding CreateBinding()
        {
            var binding = new WSHttpBinding(SecurityMode.Message)
            {
                MaxReceivedMessageSize = int.MaxValue,
                ReaderQuotas = { MaxStringContentLength = int.MaxValue },
                SendTimeout = TimeSpan.FromSeconds(30),
                OpenTimeout = TimeSpan.FromSeconds(30),
                CloseTimeout = TimeSpan.FromSeconds(30),
                ReceiveTimeout = TimeSpan.FromSeconds(30)
            };
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;

            return binding;
        }
    }
}
