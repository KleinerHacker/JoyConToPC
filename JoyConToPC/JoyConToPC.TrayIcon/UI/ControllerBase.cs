using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JoyConToPC.TrayIcon.UI
{
    internal abstract class ControllerBase<T> : INotifyPropertyChanged where T : ModelBase
    {
        public T Model { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChange([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
