using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Core.Util.Extension;
using JoyConToPC.Input;
using JoyConToPC.Input.Type;
using JoyConToPC.VJoy;
using JoyConToPC.VJoy.Type;
using log4net;

namespace JoyConToPC.Core
{
    internal class JoyConHandler
    {
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(JoyConHandler));

        private readonly JoyConManager _manager;

        private readonly IDictionary<IJoyCon, VirtualJoystick> _virtualJoystickDict =
            new Dictionary<IJoyCon, VirtualJoystick>();

        public JoyConHandler(JoyConManager manager)
        {
            _manager = manager;
            _manager.JoyConUpdated += OnJoyConUpdate;
        }

        private void OnJoyConUpdate(object sender, JoyConUpdateEventArgs args)
        {
            switch (args.Type)
            {
                case JoyConUpdateType.Connected:
                    OnNewJoyCon(args.JoyCon);
                    break;
                case JoyConUpdateType.Disconnected:
                    OnRemoveJoyCon(args.JoyCon);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void OnNewJoyCon(IJoyCon joyCon)
        {
            Logger.Info("New JoyCon " + joyCon);

            JoyConPlayer? player = JoyConPlayer.First;
            while (player != null)
            {
                if (VirtualJoystickManager.IsVirtualJoystickUsable((uint) player.Value))
                {
                    Logger.Debug(">>> Register JoyCon as " + (uint) player.Value);

                    var virtualJoystick = VirtualJoystickManager.GetVirtualJoystick((uint) player.Value);

                    joyCon.Acquire(player.Value);
                    joyCon.DataUpdated += OnJoyConDataUpdate;
                    joyCon.StartPolling();

                    _virtualJoystickDict.Add(joyCon, virtualJoystick);

                    break;
                }

                player = player.Value.Next();
            }

            if (player == null)
            {
                Logger.Warn(">>> Cannot register JoyCon: Not enought VJoy Controller found (max. 4)");
                joyCon.SetupLeds(JoyConSingleLed.Flash, JoyConSingleLed.Off, JoyConSingleLed.Off,
                    JoyConSingleLed.Flash);
            }
        }

        private void OnRemoveJoyCon(IJoyCon joyCon)
        {
            Logger.Info("Removed JoyCon " + joyCon);

            if (joyCon.IsPolling)
            {
                joyCon.StopPolling();
            }
            joyCon.DataUpdated -= OnJoyConDataUpdate;
            if (joyCon.IsAcquired)
            {
                joyCon.Unacquire();
            }

            _virtualJoystickDict[joyCon].Dispose();
            _virtualJoystickDict.Remove(joyCon);
        }

        private void OnJoyConDataUpdate(object sender, JoyConDataUpdateEventArgs args)
        {
            if (!_virtualJoystickDict.ContainsKey(args.JoyConSource))
                return;

            var virtualJoystick = _virtualJoystickDict[args.JoyConSource];
            virtualJoystick.SendData(args.JoyConState.ToVirtualJoystickData(args.JoyConSource is JoyConPair));
        }
    }
}