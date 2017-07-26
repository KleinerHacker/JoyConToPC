using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.Service
{
    [RunInstaller(true)]
    public class JoyConServiceInstaller : Installer
    {
        private ServiceInstaller _serviceInstaller;
        private ServiceProcessInstaller _serviceProcessInstaller;

        public JoyConServiceInstaller()
        {
            _serviceInstaller = new ServiceInstaller
            {
                ServiceName = "JoyCon Service",
                StartType = ServiceStartMode.Manual
            };
            _serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.NetworkService
            };

            Installers.Add(_serviceInstaller);
            Installers.Add(_serviceProcessInstaller);
        }
    }
}
