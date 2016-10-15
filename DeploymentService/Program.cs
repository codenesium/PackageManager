using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentService
{
    internal static class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            try
            {
#if (DEBUG)

                var service = new DeploymentService();
                var serviceTask = Task.Run(() => service.ConsoleStart());
                Task.WaitAll(serviceTask);

#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DeploymentService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}