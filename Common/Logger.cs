using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class LogHelper
    {
        public static ILog Logger;
        public static void LogInfo(string message)
        {
            Logger.Info(message);
        }
    }
}
