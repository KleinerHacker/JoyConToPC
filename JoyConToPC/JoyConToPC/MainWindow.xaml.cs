using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JoyConToPC.Core;
using JoyConToPC.Input;

namespace JoyConToPC
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (sender, args) => JoyConDriver.Instance.JoyConUpdated += InstanceOnJoyConUpdated;
            Unloaded += (sender, args) => JoyConDriver.Instance.JoyConUpdated -= InstanceOnJoyConUpdated;
        }

        private void InstanceOnJoyConUpdated(object sender, JoyConUpdateEventArgs joyConUpdateEventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                ListView.Items.Clear();
                var joyConList = JoyConDriver.Instance.JoyConList;
                foreach (var joyCon in joyConList)
                {
                    ListView.Items.Add(joyCon);
                }
            });
        }
    }
}