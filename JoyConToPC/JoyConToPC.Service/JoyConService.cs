using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Core;
using JoyConToPC.Service.WCF;
using log4net;

namespace JoyConToPC.Service
{
    public partial class JoyConService : ServiceBase
    {
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(JoyConService));

        private JoyConDriver _driver;
        private ServiceHost _connectorHost;

        public JoyConService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Startup JoyCon Service...");

            Logger.Debug("Startup JoyCon Driver");
            _driver = JoyConDriver.Instance;

            Logger.Debug("Startup JoyCon Driver Connector");
            _connectorHost = ServiceFactory.CreateJoyConDriverConnector();
            _connectorHost.Open();
        }

        protected override void OnStop()
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
