using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ScaffGuard
{
  public static class PrintService
  {
    static PrintService() { QuestPDF.Settings.License = LicenseType.Community; }

    public static void ExportTasksToPdf(string moduleTitle, IEnumerable<TaskItem> tasks)
    {
      var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
      Directory.CreateDirectory(dir);
      var path = Path.Combine(dir, $"{moduleTitle}-{DateTime.Now:yyyyMMdd-HHmm}.pdf");
      BuildDoc(moduleTitle, tasks).GeneratePdf(path);
      try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); } catch {}
    }

    public static void PrintTasks(string moduleTitle, IEnumerable<TaskItem> tasks)
    {
      var path = Path.Combine(Path.GetTempPath(), $"ScaffGuard-{Guid.NewGuid()}.pdf");
      BuildDoc(moduleTitle, tasks).GeneratePdf(path);
      try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true, Verb = "print" }); } catch {}
    }

    private static Document BuildDoc(string moduleTitle, IEnumerable<TaskItem> tasks) => Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Margin(30);
        page.Header().Text($"گزارش ماژول: {moduleTitle}").FontSize(20).SemiBold();
        page.Content().Table(table =>
        {
          table.ColumnsDefinition(cols =>
          {
            cols.RelativeColumn(2);
            cols.RelativeColumn(4);
            cols.RelativeColumn(2);
            cols.RelativeColumn(1);
          });
          table.Header(header =>
          {
            header.Cell().Text("عنوان").SemiBold();
            header.Cell().Text("توضیحات").SemiBold();
            header.Cell().Text("سررسید (شمسی)").SemiBold();
            header.Cell().Text("انجام شد").SemiBold();
          });
          foreach (var t in tasks)
          {
            table.Cell().Text(t.Title);
            table.Cell().Text(t.Description);
            table.Cell().Text(t.DueDateJalali);
            table.Cell().Text(t.IsDone ? "بله" : "خیر");
          }
        });
        page.Footer().AlignRight().Text($"تاریخ چاپ: {PersianDateHelper.ToJalaliDate(TehranTimeProvider.Now().Date)}");
      });
    });
  }
}
