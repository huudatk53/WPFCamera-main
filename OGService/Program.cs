using Common;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace OGService
{
    class Program
    {
        static System.Type className = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
        
       
        static void Main(string[] args)
        {

            LogHelper.Logger = LogManager.GetLogger("MyFileAppender");
            LogHelper.LogInfo("Start");
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            
            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.SetServiceName("OGService");
                hostConfigurator.SetDisplayName("OGService");
                hostConfigurator.SetDescription("OneGateMB Service");
                hostConfigurator.StartAutomatically();
                hostConfigurator.RunAsLocalSystem();
                hostConfigurator.UseLog4Net();

                hostConfigurator.Service<ProcessRunner>(s =>
                {
                    s.ConstructUsing(name => new ProcessRunner(
                        new List<IProcessService>
                        {
                            new ProcessAutoUpdate()
                        }, token));
                    s.WhenStarted(_ => _.Start());
                    s.WhenStopped(_ => _.Stop());
                });
                hostConfigurator.OnException(ex => { 
                    //log
                });
            });
        }
    }
}
