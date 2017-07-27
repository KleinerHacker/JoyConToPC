using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Core;
using JoyConToPC.Service.Base.WCF;
using log4net;
using log4net.Repository.Hierarchy;

namespace JoyConToPC.Service.Base
{
    public sealed class JoyConServiceWrapper
    {
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(JoyConServiceWrapper));

        private JoyConDriver _driver;
        private ServiceHost _connectorHost;

        public void StartServiceWrapper()
        {
            Logger.Info("Startup JoyCon Service...");

            Logger.Debug("Startup JoyCon Driver");
            _driver = JoyConDriver.Instance;

            Logger.Debug("Startup JoyCon Driver Connector");
            _connectorHost = ServiceFactory.CreateJoyConDriverConnector();
            _connectorHost.Open();
        }

        public void StopServiceWrapper()
        {
            Logger.Info("Stop JoyCon Service...");

            Logger.Debug("Stop JoyCon Driver");
            _driver = null;

            Logger.Debug("Stop JoyCon Driver Connector");
            _connectorHost.Close();
            _connectorHost = null;
        }
    }
}
