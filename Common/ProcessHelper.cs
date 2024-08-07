using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ProcessHelper
    {
        public static void InitFunction(Action func, string functionname)
        {
            //LogService.Info($"InitFunction {func.Target.ToString()} {functionname}");
            Task.Run(func);
        }
    }
}
