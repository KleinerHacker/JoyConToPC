using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace JoyConToPC.VJoy.Util
{
    internal static class VJoyConfig
    {
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(VJoyConfig));

        public static void CreateDevice(uint id, VJoyDeviceProfile profile)
        {
            Logger.Info("Create VJoy " + id + " based on profile " + profile);

            Process process;
            switch (profile)
            {
                case VJoyDeviceProfile.Small:
                    process = new Process
                    {
                        StartInfo = new ProcessStartInfo("vJoyConfig.exe", id + " -f -a x y -b 10")
                        {
                            Verb = "runas",
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            UseShellExecute = true
                        }
                    };

                    break;
                case VJoyDeviceProfile.Full:
                    process = new Process
                    {
                        StartInfo = new ProcessStartInfo("vJoyConfig.exe", id + " -f - a x y z -b 10 -p 4")
                        {
                            Verb = "runas",
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            UseShellExecute = true
                        }
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }

            process.Start();
            if (!process.WaitForExit(10000))
            {
                process.Kill();
            }
            if (process.ExitCode != 0)
            {
                Logger.Warn("Exit code is not 0. It was " + process.ExitCode);
            }
            else
            {
                Logger.Info("Creation of VJoy " + id + " based on profile " + profile + " successfully");
            }
        }

        public static void DeleteDevice(uint id)
        {
            Logger.Info("Delete VJoy " + id);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo("vJoyConfig.exe", "-d " + id)
                {
                    Verb = "runas",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true
                }
            };
            process.Start();
            if (!process.WaitForExit(10000))
            {
                process.Kill();
            }
            if (process.ExitCode != 0)
            {
                Logger.Warn("Exit code is not 0. It was " + process.ExitCode);
            }
            else
            {
                Logger.Info("Deleting of VJoy " + id + " successfully");
            }
        }
    }

    public enum VJoyDeviceProfile
    {
        Small,
        Full
    }
}