using System;
using System.Globalization;
using Varych.TaskPlanner.Domain.Models1;
using Varych.TaskPlanner.Domain.Models1.Enum;
using Varych.TaskPlanner.Domain.Logic1;
using Varych.TaskPlanner.DataAccess;
using Varych.TaskPlanner.DataAccess.Abstractions;

namespace Varych.TaskPlanner
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IWorkItemsRepository repo = new FileWorkItemsRepository();
            var planner = new SimpleTaskPlanner(repo); 

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== Task Planner ===");
                Console.WriteLine("[A]dd  [B]uild plan  [M]ark completed  [R]emove  [Q]uit");
                Console.Write("Choose: ");
                var cmd = Console.ReadLine()?.Trim();

                switch (cmd?.ToUpperInvariant())
                {
                    case "A":
                    case "ADD":
                        AddWorkItem(repo);
                        break;

                    case "B":
                    case "BUILD":
                        BuildPlan(repo, planner);
                        break;

                    case "M":
                    case "MARK":
                        MarkCompleted(repo);
                        break;

                    case "R":
                    case "REMOVE":
                        RemoveItem(repo);
                        break;

                    case "Q":
                    case "QUIT":
                        repo.SaveChanges();
                        Console.WriteLine("Зміни збережено. Вихід...");
                        return;

                    default:
                        Console.WriteLine("Невідома команда.");
                        break;
                }
            }
        }

        private static void AddWorkItem(IWorkItemsRepository repo)
        {
            Console.Write("Title: ");
            var title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Назва не може бути порожньою.");
                return;
            }

            Console.Write("Description: ");
            var description = Console.ReadLine();

            Console.Write("Due date (dd.MM.yyyy, Enter = без дедлайну): ");
            DateTime? dueDate = null;
            var dueStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dueStr))
            {
                if (DateTime.TryParseExact(dueStr, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var parsed))
                {
                    dueDate = parsed;
                }
                else
                {
                    Console.WriteLine("Невірний формат дати. Спробуйте ще раз.");
                    return;
                }
            }

            Console.Write("Priority (None, Low, Medium, High, Urgent): ");
            if (!Enum.TryParse<Priority>(Console.ReadLine(), true, out var priority))
            {
                Console.WriteLine("Невірний пріоритет.");
                return;
            }

            Console.Write("Complexity (None, Minutes, Hours, Days, Weeks): ");
            if (!Enum.TryParse<Complexity>(Console.ReadLine(), true, out var complexity))
            {
                Console.WriteLine("Невірна складність.");
                return;
            }

            var wi = new WorkItem(title, description, dueDate ?? default, priority, complexity)
            {
                DueDate = dueDate ?? DateTime.MaxValue
            };

            try { wi.IsCompleted = false; } catch { /* якщо поля немає, ігноруємо */ }

            var id = repo.Add(wi);
            repo.SaveChanges();
            Console.WriteLine($"Додано. Id = {id}");
        }

        private static void BuildPlan(IWorkItemsRepository repo, SimpleTaskPlanner planner)
        {
            var all = repo.GetAll();
            if (all.Length == 0)
            {
                Console.WriteLine("Порожньо.");
                return;
            }

            var plan = planner.CreatePlan();

            Console.WriteLine("\n=== План виконання ===");
            foreach (var w in plan)
            {
                var status = GetStatus(w);
                var due = (w.DueDate == DateTime.MaxValue) ? "—" : w.DueDate.ToString("dd.MM.yyyy");
                Console.WriteLine($"{status} {w.Title} | Due: {due} | Pri:{w.Priority} | Cmplx:{w.Complexity} | Id:{w.Id}");
            }
        }

        private static void MarkCompleted(IWorkItemsRepository repo)
        {
            var all = repo.GetAll();
            if (all.Length == 0)
            {
                Console.WriteLine("Немає елементів.");
                return;
            }

            ListAllBrief(all);

            Console.Write("Введіть Id для відмітки як виконаного: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Невірний формат Id.");
                return;
            }

            var item = repo.Get(id);
            if (item == null)
            {
                Console.WriteLine("Не знайдено.");
                return;
            }

            try
            {
                item.IsCompleted = true;
            }
            catch
            {
                Console.WriteLine("У моделі WorkItem немає поля IsCompleted. Додайте його: public bool IsCompleted { get; set; }");
                return;
            }

            if (repo.Update(item))
            {
                repo.SaveChanges();
                Console.WriteLine("Позначено як виконаний.");
            }
            else Console.WriteLine("Не вдалося оновити.");
        }

        private static void RemoveItem(IWorkItemsRepository repo)
        {
            var all = repo.GetAll();
            if (all.Length == 0)
            {
                Console.WriteLine("Немає елементів.");
                return;
            }

            ListAllBrief(all);

            Console.Write("Введіть Id для видалення: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Невірний формат Id.");
                return;
            }

            if (repo.Remove(id))
            {
                repo.SaveChanges();
                Console.WriteLine("Видалено.");
            }
            else Console.WriteLine("Не знайдено.");
        }

        private static void ListAllBrief(WorkItem[] items)
        {
            Console.WriteLine("Поточні задачі:");
            foreach (var w in items)
            {
                var status = GetStatus(w);
                var due = (w.DueDate == DateTime.MaxValue) ? "—" : w.DueDate.ToString("dd.MM.yyyy");
                Console.WriteLine($"{status} {w.Title} | Due: {due} | Pri:{w.Priority} | Cmplx:{w.Complexity} | Id:{w.Id}");
            }
        }

        private static string GetStatus(WorkItem w)
        {
            try { return w.IsCompleted ? "[+]" : "[ ]"; }
            catch { return "[ ]"; }
        }
    }
}
