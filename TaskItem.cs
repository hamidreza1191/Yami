using System;

namespace ScaffGuard
{
  public class TaskItem
  {
    public string Id { get; set; } = "";
    public string ModuleId { get; set; } = "";
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDateUtc { get; set; }
    public bool IsDone { get; set; }

    public string DueDateJalali
    {
      get => PersianDateHelper.ToJalaliDate(TehranTimeProvider.ConvertFromUtc(DueDateUtc).Date);
      set
      {
        var parsed = PersianDateHelper.ParseJalaliDate(value);
        DueDateUtc = TehranTimeProvider.ToUtc(parsed);
      }
    }
  }
}
