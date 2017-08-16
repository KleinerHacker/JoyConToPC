using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using JoyConToPC.TrayIcon.UI.Window;
using Application = System.Windows.Application;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;

namespace JoyConToPC.TrayIcon
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private readonly NotifyIcon _notifyIcon;

        public App()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = TrayIcon.Properties.Resources.CP050,
                Text = "JoyCon Driver",
                ContextMenu = new ContextMenu(new [] {
                    new MenuItem("Open JoyCon List...", (sender, args) => new JoyConOverviewWindow().Show()), 
                    new MenuItem("-"), 
                    new MenuItem("Exit", (sender, args) => Shutdown(0)),  
                })
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _notifyIcon.Visible = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Visible = false;
            base.OnExit(e);
        }
    }
}