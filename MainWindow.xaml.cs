using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using LiteDB;

namespace ScaffGuard
{
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    private readonly StorageService _storage;
    private readonly SchedulerService _scheduler;
    public ObservableCollection<ModuleViewModel> Modules { get; } = new();
    public ObservableCollection<TaskItem> FilteredTasks { get; } = new();
    public event PropertyChangedEventHandler? PropertyChanged;

    private ModuleViewModel? _selectedModule;
    public string SearchText { get; set; } = string.Empty;

    public string NowTehran => TehranTimeProvider.Now().ToString("HH:mm:ss");
    public string TodayJalali => PersianDateHelper.ToJalaliDate(TehranTimeProvider.Now().Date);

    public MainWindow()
    {
      InitializeComponent();
      DataContext = this;

      _storage = new StorageService();
      _scheduler = new SchedulerService(NotifyDueTask);
      LoadData();
      StartClock();
    }

    private void StartClock()
    {
      var t = new Timer(1000);
      t.Elapsed += (s, e) => Dispatcher.Invoke(() => OnPropertyChanged(nameof(NowTehran)));
      t.Start();
    }

    void LoadData()
    {
      foreach (var module in _storage.LoadModules())
      {
        var vm = new ModuleViewModel(module, _storage);
        vm.PropertyChanged += (_, __) => RefreshBadges();
        Modules.Add(vm);
      }
      if (Modules.Count == 0)
      {
        var vm = new ModuleViewModel(new Module { Id = Guid.NewGuid().ToString(), Title = "ماژول نمونه" }, _storage);
        Modules.Add(vm);
        _storage.SaveModule(vm.Model);
      }
      SelectModule(Modules.First().Id);
    }

    void NotifyDueTask(TaskItem task)
    {
      // In-app notification (Windows Toast can be added when packaging)
      Dispatcher.Invoke(() =>
      {
        MessageBox.Show($"سررسید انجام کار: {task.Title}", "یادآوری", MessageBoxButton.OK, MessageBoxImage.Information);
      });
    }

    void RefreshBadges()
    {
      foreach (var m in Modules) m.UpdateDueCount();
    }

    public void SelectModule(string id)
    {
      foreach (var m in Modules) m.IsSelected = (m.Id == id);
      _selectedModule = Modules.First(m => m.Id == id);
      ApplyFilter();
      RefreshBadges();
    }

    void ApplyFilter()
    {
      FilteredTasks.Clear();
      foreach (var t in _storage.LoadTasks(_selectedModule!.Id)
        .Where(t => string.IsNullOrWhiteSpace(SearchText) || (t.Title?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)))
      {
        FilteredTasks.Add(t);
      }
    }

    // Commands (simplified as event handlers)
    public RelayCommand AddModuleCommand => new(() =>
    {
      var vm = new ModuleViewModel(new Module { Id = Guid.NewGuid().ToString(), Title = "ماژول جدید" }, _storage);
      Modules.Add(vm);
      _storage.SaveModule(vm.Model);
    });

    public RelayCommand ResetAllCommand => new(() =>
    {
      _storage.ResetAll();
      foreach (var m in Modules) m.UpdateDueCount();
      ApplyFilter();
    });

    public RelayCommand AddTaskCommand => new(() =>
    {
      if (_selectedModule is null) return;
      var task = new TaskItem
      {
        Id = Guid.NewGuid().ToString(),
        ModuleId = _selectedModule.Id,
        Title = "کار جدید",
        Description = "",
        DueDateUtc = TehranTimeProvider.Now().AddDays(1).ToUniversalTime(),
        IsDone = false
      };
      _storage.SaveTask(task);
      ApplyFilter();
      RefreshBadges();
      _scheduler.Schedule(task);
    });

    public RelayCommand ExportPdfCommand => new(() =>
    {
      PrintService.ExportTasksToPdf(_selectedModule!.Title, _storage.LoadTasks(_selectedModule!.Id));
      MessageBox.Show("فایل PDF گزارش در کنار برنامه ساخته شد (Reports).");
    });

    public RelayCommand PrintReportCommand => new(() =>
    {
      PrintService.PrintTasks(_selectedModule!.Title, _storage.LoadTasks(_selectedModule!.Id));
    });

    public RelayCommand ResetModuleCommand => new(() =>
    {
      if (_selectedModule is null) return;
      _storage.ResetModule(_selectedModule.Id);
      ApplyFilter();
      RefreshBadges();
    });

    public RelayCommand SelectModuleCommand => new(obj =>
    {
      var id = obj?.ToString() ?? "";
      SelectModule(id);
    });

    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
  }

  public class RelayCommand : System.Windows.Input.ICommand
  {
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool> _can;
    public RelayCommand(Action execute) : this(_ => execute(), _ => true) { }
    public RelayCommand(Action<object?> execute) : this(execute, _ => true) { }
    public RelayCommand(Action<object?> execute, Func<object?, bool> can)
    {
      _execute = execute; _can = can;
    }
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => _can(parameter);
    public void Execute(object? parameter) => _execute(parameter);
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
  }
}
