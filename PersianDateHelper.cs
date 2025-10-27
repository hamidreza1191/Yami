using System;
using System.Globalization;

namespace ScaffGuard
{
  public static class PersianDateHelper
  {
    private static readonly PersianCalendar pc = new PersianCalendar();

    public static string ToJalaliDate(DateTime date)
    {
      var d = date;
      return $"{pc.GetYear(d):0000}/{pc.GetMonth(d):00}/{pc.GetDayOfMonth(d):00}";
    }

    public static DateTime ParseJalaliDate(string s)
    {
      // Expected format yyyy/MM/dd
      var parts = s.Split('/', '-', '.');
      int y = int.Parse(parts[0]);
      int m = int.Parse(parts[1]);
      int d = int.Parse(parts[2]);
      return new DateTime(y, m, d, pc);
    }
  }
}
