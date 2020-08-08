using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Claysys.PPP.Forgiveness
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (System.Environment.CommandLine.ToString().ToLower().Contains("--debug"))
            {
                var forgivessObj = new Claysys.PPP.Forgiveness.PPPForgiveness();
                forgivessObj.Init();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new PPPForgiveness()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
