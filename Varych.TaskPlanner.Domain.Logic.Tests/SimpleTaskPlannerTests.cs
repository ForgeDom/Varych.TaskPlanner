using System;
using System.Linq;
using Moq;
using Xunit;
using Varych.TaskPlanner.DataAccess.Abstractions;
using Varych.TaskPlanner.Domain.Logic1;
using Varych.TaskPlanner.Domain.Models1;
using Varych.TaskPlanner.Domain.Models1.Enum;

namespace YourProjectName.TaskPlanner.Domain.Logic.Tests
{
    public class SimpleTaskPlannerTests
    {
        [Fact]
        public void CreatePlan_SortsByPriorityThenDueDateThenTitle()
        {
            var items = new[]
            {
                MakeItem("U later",  Priority.Urgent, new DateTime(2025,10,20)),
                MakeItem("H later",  Priority.High,   new DateTime(2025,10,18)),
                MakeItem("H sooner", Priority.High,   new DateTime(2025,10,16)),
                MakeItem("M mid",    Priority.Medium, new DateTime(2025,10,17)),
                MakeItem("L last",   Priority.Low,    new DateTime(2025,10,19)),
                MakeItem("H sameA",  Priority.High,   new DateTime(2025,10,16)),
                MakeItem("H sameB",  Priority.High,   new DateTime(2025,10,16)),
            };

            var repo = new Mock<IWorkItemsRepository>();
            repo.Setup(r => r.GetAll()).Returns(items);

            var planner = new SimpleTaskPlanner(repo.Object);

            var plan = planner.CreatePlan();

            var expected = new[]
            {
                "U later",
                "H sameA", "H sameB", "H sooner", "H later",
                "M mid",
                "L last"
            };

            Assert.Equal(expected, plan.Select(p => p.Title).ToArray());
        }

        [Fact]
        public void CreatePlan_DoesNotFilterOutCompleted_TakesAllItems()
        {
            var items = new[]
            {
                MakeItem("Active A", Priority.High,   new DateTime(2025,10,16), isCompleted:false),
                MakeItem("Done A",   Priority.Urgent, new DateTime(2025,10,15), isCompleted:true),
                MakeItem("Active B", Priority.Low,    new DateTime(2025,10,20), isCompleted:false),
                MakeItem("Done B",   Priority.Medium, new DateTime(2025,10,18), isCompleted:true),
            };

            var repo = new Mock<IWorkItemsRepository>();
            repo.Setup(r => r.GetAll()).Returns(items);

            var planner = new SimpleTaskPlanner(repo.Object);

            var plan = planner.CreatePlan();

            Assert.Equal(items.Length, plan.Length);
            Assert.Contains(plan, p => p.Title == "Done A");
            Assert.Contains(plan, p => p.Title == "Done B");
            Assert.Contains(plan, p => p.Title == "Active A");
            Assert.Contains(plan, p => p.Title == "Active B");
            Assert.Equal("Done A", plan.First().Title);
        }

        [Fact]
        public void CreatePlan_StableOrderingOnSamePriorityAndDueDate_UsesTitleAscending()
        {
            var items = new[]
            {
                MakeItem("bbb", Priority.Medium, new DateTime(2025,10,17)),
                MakeItem("aaa", Priority.Medium, new DateTime(2025,10,17)),
                MakeItem("ccc", Priority.Medium, new DateTime(2025,10,17)),
            };

            var repo = new Mock<IWorkItemsRepository>();
            repo.Setup(r => r.GetAll()).Returns(items);

            var planner = new SimpleTaskPlanner(repo.Object);

            var plan = planner.CreatePlan();

            Assert.Equal(new[] { "aaa", "bbb", "ccc" }, plan.Select(p => p.Title).ToArray());
        }

        private static WorkItem MakeItem(string title, Priority p, DateTime due, bool isCompleted = false)
        {
            var wi = new WorkItem(title, $"desc {title}", due, p, Complexity.Hours);
            wi.IsCompleted = isCompleted;
            return wi;
        }
    }
}
