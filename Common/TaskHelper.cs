using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static class TaskHelper
    {
        public static List<TaskInfo> ListTask = new List<TaskInfo>();
        public static TaskInfo CreateAndRunTask(Guid id)
        {
            var exist = ListTask.FirstOrDefault(x => x.id == id);
            if (exist == null)
            {
                var task = new TaskInfo()
                {
                    id = id,
                    source = new CancellationTokenSource(),
                    status = "running"
                };
                ListTask.Add(task);
                return task;
            }
            else return exist;

        }
        public static TaskInfo GetTaskById(Guid id)
        {
            return ListTask.FirstOrDefault(x => x.id == id);
        }
        public static void RemoveTaskById(Guid id)
        {
            var task = ListTask.FirstOrDefault(x => x.id == id);
            if (task != null)
            {
                ListTask.Remove(task);
            }
            
        }
        public static List<TaskInfo> GetList()
        {
            return ListTask;
        }
    }

    public class TaskInfo
    {
        public Guid id { get; set; }

        public string status { get; set; }
        public CancellationTokenSource source { get; set; }
    }
}
