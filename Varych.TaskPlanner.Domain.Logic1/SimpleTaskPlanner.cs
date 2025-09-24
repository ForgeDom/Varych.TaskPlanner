using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varych.TaskPlanner.Domain.Models1;

namespace Varych.TaskPlanner.Domain.Logic1
{
    public class SimpleTaskPlanner
    {
        public WorkItem[] CreatePlan(WorkItem[] items)
        {
            List<WorkItem> workItems = items.ToList();
            workItems.Sort((x, y) =>
            {
                int result = y.Priority.CompareTo(x.Priority);
                if (result == 0)
                {
                    result = x.DueDate.CompareTo(y.DueDate);
                }
                if (result == 0)
                {
                    result = string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
                }
                return result;
            });

            return workItems.ToArray();
        }

    }
}
