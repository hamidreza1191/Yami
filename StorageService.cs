using System;
using System.Collections.Generic;
using System.IO;
using LiteDB;

namespace ScaffGuard
{
  public class StorageService
  {
    private readonly string _dbPath;
    public StorageService()
    {
      var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScaffGuard");
      Directory.CreateDirectory(baseDir);
      _dbPath = Path.Combine(baseDir, "scaffguard.db");
    }

    public IEnumerable<Module> LoadModules()
    {
      using var db = new LiteDatabase(_dbPath);
      var col = db.GetCollection<Module>("modules");
      col.EnsureIndex(x => x.Id, true);
      return col.FindAll();
    }

    public void SaveModule(Module m)
    {
      using var db = new LiteDatabase(_dbPath);
      var col = db.GetCollection<Module>("modules");
      col.Upsert(m);
    }

    public IEnumerable<TaskItem> LoadTasks(string moduleId)
    {
      using var db = new LiteDatabase(_dbPath);
      var col = db.GetCollection<TaskItem>("tasks");
      col.EnsureIndex(x => x.ModuleId);
      return col.Find(x => x.ModuleId == moduleId);
    }

    public void SaveTask(TaskItem t)
    {
      using var db = new LiteDatabase(_dbPath);
      var col = db.GetCollection<TaskItem>("tasks");
      col.Upsert(t);
    }

    public void ResetModule(string moduleId)
    {
      using var db = new LiteDatabase(_dbPath);
      var col = db.GetCollection<TaskItem>("tasks");
      col.DeleteMany(x => x.ModuleId == moduleId);
    }

    public void ResetAll()
    {
      using var db = new LiteDatabase(_dbPath);
      db.DropCollection("tasks");
    }
  }
}
