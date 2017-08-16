using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using JoyConToPC.API.Type;
using JoyConToPC.TrayIcon.WCF;

namespace JoyConToPC.TrayIcon.UI.Window
{
    internal sealed class JoyConOverviewWindowModel : ModelBase
    {
        public List<CJoyCon> JoyConList { get; } = new List<CJoyCon>();

        public JoyConOverviewWindowModel()
        {
            new Timer { Enabled = true, Interval = 1000}.Elapsed += OnElapsed;
        }

        private void OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            JoyConList.Clear();
            JoyConList.AddRange(JoyConDriverConnectorClient.Instance.GetJoyConList());
        }
    }
}
