using System;
using System.Collections.Generic;
using System.Linq;
using Varych.TaskPlanner.DataAccess.Abstractions;   // IWorkItemsRepository
using Varych.TaskPlanner.Domain.Models1;

namespace Varych.TaskPlanner.Domain.Logic1
{
    public class SimpleTaskPlanner
    {
        private readonly IWorkItemsRepository _repo;

        public SimpleTaskPlanner(IWorkItemsRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public WorkItem[] CreatePlan()
        {
            var items = _repo.GetAll() ?? Array.Empty<WorkItem>();
            var list = new List<WorkItem>(items);

            list.Sort((x, y) =>
            {
                int result = y.Priority.CompareTo(x.Priority);

                if (result == 0)
                {
                    var xDue = x.DueDate;
                    var yDue = y.DueDate;

                    result = xDue.CompareTo(yDue);
                }

                if (result == 0)
                    result = string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);

                return result;
            });

            return list.ToArray();
        }
    }
}
