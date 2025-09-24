using System;
using Varych.TaskPlanner.Domain.Models1;
using Varych.TaskPlanner.Domain.Models1.Enum;
using Varych.TaskPlanner.Domain.Logic1;
namespace Varych.TaskPlanner
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            List<WorkItem> items = new List<WorkItem>();

            Console.WriteLine("Enter WorkItems(leave empty to finish):");

            while (true)
            {
                Console.Write("Title: ");
                string title = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(title)) break;

                Console.Write("Description: ");
                string description = Console.ReadLine();

                Console.Write("Due date (dd.MM.yyyy): ");
                DateTime dueDate = DateTime.Parse(Console.ReadLine());

                Console.Write("Priority (None,Low,Medium,High,Urgent): ");
                Priority priority = Enum.Parse<Priority>(Console.ReadLine(), true);

                Console.Write("Complexity (None,Minutes,Hours,Days,Weeks): ");
                Complexity complexity = Enum.Parse<Complexity>(Console.ReadLine(), true);

                items.Add(new WorkItem(title, description, dueDate, priority, complexity));
                Console.WriteLine("WorkItem added!\n");
            }

            SimpleTaskPlanner planner = new SimpleTaskPlanner();
            WorkItem[] sortedItems = planner.CreatePlan(items.ToArray());

            Console.WriteLine("\nSorted WorkItems:");
            foreach (var item in sortedItems)
            {
                Console.WriteLine(item);
            }
        }
    }
}
