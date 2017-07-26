using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Common.Type;
using JoyConToPC.Input;
using JoyConToPC.Input.Type;

namespace JoyConToPC.Core
{
    public sealed class JoyConDriver
    {
        public static JoyConDriver Instance { get; } = new JoyConDriver();

        private readonly JoyConManager _manager;
        private readonly JoyConHandler _handler;

        private JoyConDriver()
        {
            _manager = new JoyConManager();
            _handler = new JoyConHandler(_manager);
        }

        ~JoyConDriver()
        {
            _manager.Dispose();
        }

        #region Delegates Manager

        public IReadOnlyCollection<IJoyCon> JoyConList => _manager.JoyConList;

        public event EventHandler<JoyConUpdateEventArgs> JoyConUpdated
        {
            add { _manager.JoyConUpdated += value; }
            remove { _manager.JoyConUpdated -= value; }
        }

        #endregion
    }
}
