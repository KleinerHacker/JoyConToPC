using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JoyConToPC.TrayIcon.Properties;

namespace JoyConToPC.TrayIcon
{
    internal class JoyConApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;

        public JoyConApplicationContext()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = Resources.CP050,
                Text = "JoyCon Driver",
                ContextMenu = new ContextMenu(new[] {
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _notifyIcon.Visible = false;

            Application.Exit();
        }
    }
}
