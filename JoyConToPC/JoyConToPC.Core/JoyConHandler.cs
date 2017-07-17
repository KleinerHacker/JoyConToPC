﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Input.Type;

namespace JoyConToPC.Core
{
    internal class JoyConHandler
    {
        private readonly JoyConManager _manager;

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
            joyCon.Acquire(JoyConPlayer.First);
            joyCon.DataUpdated += OnJoyConDataUpdate;
            joyCon.StartPolling();
        }

        private void OnRemoveJoyCon(IJoyCon joyCon)
        {
            if (joyCon.IsPolling)
            {
                joyCon.StopPolling();
            }
            joyCon.DataUpdated -= OnJoyConDataUpdate;
            if (joyCon.IsAcquired)
            {
                joyCon.Unacquire();
            }
        }

        private void OnJoyConDataUpdate(object sender, JoyConDataUpdateEventArgs args)
        {
            Console.WriteLine(args.JoyConState);
        }
    }
}