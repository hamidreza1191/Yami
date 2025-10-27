using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace ScaffGuard
{
  public class SchedulerService
  {
    private readonly Timer _timer;
    private readonly Action<TaskItem> _notify;
    private readonly List<TaskItem> _tasks = new();

    public SchedulerService(Action<TaskItem> notify)
    {
      _notify = notify;
      _timer = new Timer(30_000); // check every 30 sec
      _timer.Elapsed += (_, __) => CheckDue();
      _timer.Start();
    }

    public void Schedule(TaskItem item)
    {
      if (_tasks.All(t => t.Id != item.Id))
        _tasks.Add(item);
    }

    private void CheckDue()
    {
      var nowUtc = DateTime.UtcNow;
      foreach (var t in _tasks.Where(t => !t.IsDone && t.DueDateUtc <= nowUtc).ToList())
      {
        _notify(t);
        _tasks.Remove(t);
      }
    }
  }
}
