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
using JoyConToPC.Service.Base;
using log4net;
using log4net.Config;

namespace JoyConToPC.Service
{
    public partial class JoyConService : ServiceBase
    {
        private readonly JoyConServiceWrapper _serviceWrapper = new JoyConServiceWrapper();

        public JoyConService()
        {
            InitializeComponent();
            BasicConfigurator.Configure();
        }

        protected override void OnStart(string[] args)
        {
            _serviceWrapper.StartServiceWrapper();
        }

        protected override void OnStop()
        {
            _serviceWrapper.StopServiceWrapper();
        }
    }
}
