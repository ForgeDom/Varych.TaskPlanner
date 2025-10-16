using Varych.TaskPlanner.Domain.Models1;
using Varych.TaskPlanner.DataAccess.Abstractions;
using Newtonsoft.Json;


namespace Varych.TaskPlanner.DataAccess
{
    public class FileWorkItemsRepository : IWorkItemsRepository
    {
        private const string FileName = "work-items.json";
        private readonly Dictionary<Guid, WorkItem> _workItems;

        public FileWorkItemsRepository()
        {
            if (File.Exists(FileName) && new FileInfo(FileName).Length > 0)
            {
                try
                {
                    string json = File.ReadAllText(FileName);
                    var items = JsonConvert.DeserializeObject<WorkItem[]>(json);

                    if (items != null)
                    {
                        _workItems = new Dictionary<Guid, WorkItem>();
                        foreach (var item in items)
                        {
                            _workItems[item.Id] = item;
                        }
                    }
                    else
                    {
                        _workItems = new Dictionary<Guid, WorkItem>();
                    }
                }
                catch
                {
                    _workItems = new Dictionary<Guid, WorkItem>();
                }
            }
            else
            {
                _workItems = new Dictionary<Guid, WorkItem>();
            }
        }

        public Guid Add(WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            var copy = new WorkItem
            {
                Id = Guid.NewGuid(),
                Title = workItem.Title,
                Description = workItem.Description,
                DueDate = workItem.DueDate,
                Priority = workItem.Priority,
                Complexity = workItem.Complexity
            };

            _workItems[copy.Id] = copy;
            return copy.Id;
        }

        public WorkItem Get(Guid id)
        {
            _workItems.TryGetValue(id, out var item);
            return item;
        }

        public WorkItem[] GetAll()
        {
            var values = new List<WorkItem>(_workItems.Values);
            return values.ToArray();
        }

        public bool Update(WorkItem workItem)
        {
            if (workItem == null || !_workItems.ContainsKey(workItem.Id))
                return false;

            _workItems[workItem.Id] = workItem;
            return true;
        }

        public bool Remove(Guid id)
        {
            return _workItems.Remove(id);
        }

        public void SaveChanges()
        {
            var items = new List<WorkItem>(_workItems.Values);
            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }
    }
}
