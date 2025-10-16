using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varych.TaskPlanner.Domain.Models1.Enum;

namespace Varych.TaskPlanner.Domain.Models1
{
    public class WorkItem
    {
        public WorkItem()
        {

        }

        public WorkItem(string title, string? description, DateTime dueDate, Priority priority, Complexity complexity)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Complexity = complexity;

        }

        public DateTime DateTime { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public Complexity Complexity { get; set; }
        public bool IsCompleted { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }

        public WorkItem Clone()
        {
            return new WorkItem
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                DateTime = this.DateTime,
                DueDate = this.DueDate,
                Priority = this.Priority,
                Complexity = this.Complexity,
                IsCompleted = this.IsCompleted
            };
        }
        
        public override string ToString()
        {
            return $"{Title}: due {DueDate:dd.MM.yyyy}, {Priority.ToString().ToLower()} priority";
        }
    }
}
