using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ScaffGuard
{
  public class ModuleViewModel : INotifyPropertyChanged
  {
    private readonly StorageService _storage;
    public Module Model { get; }
    public string Id => Model.Id;
    public string Title { get => Model.Title; set { Model.Title = value; _storage.SaveModule(Model); OnPropertyChanged(); } }

    public int DueCount { get; private set; }
    public Visibility DueCountVisibility => DueCount > 0 ? Visibility.Visible : Visibility.Collapsed;
    public bool IsSelected { get; set; }

    public ModuleViewModel(Module model, StorageService storage)
    {
      Model = model; _storage = storage;
      UpdateDueCount();
    }

    public void UpdateDueCount()
    {
      int count = 0;
      foreach (var t in _storage.LoadTasks(Model.Id))
        if (!t.IsDone && t.DueDateUtc <= DateTime.UtcNow) count++;
      DueCount = count;
      OnPropertyChanged(nameof(DueCount));
      OnPropertyChanged(nameof(DueCountVisibility));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
  }
}
