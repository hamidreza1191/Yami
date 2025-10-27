using System;

namespace ScaffGuard
{
  public static class TehranTimeProvider
  {
    public static DateTime Now()
    {
      var tz = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
      return TimeZoneInfo.ConvertTime(DateTime.Now, tz);
    }

    public static DateTime ToUtc(DateTime tehranLocal)
    {
      var tz = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
      return TimeZoneInfo.ConvertTimeToUtc(tehranLocal, tz);
    }

    public static DateTime ConvertFromUtc(DateTime utc)
    {
      var tz = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
      return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
    }
  }
}
