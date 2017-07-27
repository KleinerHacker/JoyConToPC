using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyConToPC.Service.Base;
using log4net.Config;

namespace JoyConToPC.Service.Test
{
    class Program
    {
        private static readonly JoyConServiceWrapper ServiceWrapper = new JoyConServiceWrapper();

        static void Main(string[] args)
        {
            BasicConfigurator.Configure();

            ServiceWrapper.StartServiceWrapper();

            Console.WriteLine("Started");
            Console.ReadLine();

            ServiceWrapper.StopServiceWrapper();
        }
    }
}
