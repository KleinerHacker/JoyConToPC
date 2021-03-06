﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Common.Type;
using JoyConToPC.Core.Util.Extension;
using JoyConToPC.Input;
using JoyConToPC.Input.Type;
using JoyConToPC.VJoy;
using JoyConToPC.VJoy.Type;
using JoyConToPC.VJoy.Util;
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
                if (_virtualJoystickDict.Values.Any(vjoy => vjoy.DeviceId == (uint) player))
                {
                    player = player.Value.Next();
                    continue;
                }

                var virtualJoystick = new VirtualJoystick((uint) player.Value,
                    joyCon is JoyCon ? VJoyDeviceProfile.Small : VJoyDeviceProfile.Full);
                if (virtualJoystick.UsageState != UsageState.Free)
                {
                    virtualJoystick.Dispose();
                    player = player.Value.Next();
                    continue;
                }

                Logger.Debug(">>> Register JoyCon as " + (uint) player.Value);

                joyCon.DataUpdated += OnJoyConDataUpdate;
                joyCon.Acquire(player.Value);

                virtualJoystick.Aquire();
                _virtualJoystickDict.Add(joyCon, virtualJoystick);

                break;
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
            
            if (joyCon.IsAcquired)
            {
                joyCon.Unacquire();
            }
            joyCon.DataUpdated -= OnJoyConDataUpdate;

            Console.WriteLine($"JoyCon State: {joyCon.IsAcquired}");
            _virtualJoystickDict[joyCon].Dispose();
            _virtualJoystickDict.Remove(joyCon);
        }

        private void OnJoyConDataUpdate(object sender, JoyConDataUpdateEventArgs args)
        {
            if (!_virtualJoystickDict.ContainsKey(args.JoyConSource))
                return;

            var virtualJoystick = _virtualJoystickDict[args.JoyConSource];
            if (!virtualJoystick.IsAquired) //In case of driver restart
            {
                Logger.Debug("Re-Aquire virtual joystick " + virtualJoystick);
                if (!virtualJoystick.Aquire())
                {
                    Logger.Warn("Unable to re-aquire virtual joystick " + virtualJoystick + "; ignore data update");
                    return;
                }
            }
            virtualJoystick.SendData(args.JoyConState.ToVirtualJoystickData(args.JoyConSource));
        }
    }
}