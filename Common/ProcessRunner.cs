using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class ProcessRunner
    {
        private readonly CancellationTokenSource _cts;
        private readonly IEnumerable<IProcessService> _processes;
        public ProcessRunner(IEnumerable<IProcessService> processes, CancellationToken ct)
        {
            _processes = processes;
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        }

        public void Start()
        {
            foreach (var process in _processes)
            {
                try
                {
                    
                    Task.Run(process.ProcessInit, _cts.Token);
                }
                catch (Exception)
                {
                    
                }

            }

        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
    public interface IProcessService
    {
        string ProcessName { get; }
        void ProcessInit();
    }
}
