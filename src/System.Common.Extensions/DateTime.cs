using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.Common.Extensions
{
  public static partial class DateTimeExtensions
  {
    public const long TwoWeeksTicks = 12096000000000;
    private static readonly DateTime firstPayDay = DateTime.Parse("1/11/0001");

    private static readonly Dictionary<DayOfWeek, string> dayAbbreviations = new Dictionary<DayOfWeek, string>
    {
      {DayOfWeek.Friday, "Fri"},
      {DayOfWeek.Monday, "Mon"},
      {DayOfWeek.Saturday, "Sat"},
      {DayOfWeek.Sunday, "Sun"},
      {DayOfWeek.Thursday, "Thu"},
      {DayOfWeek.Tuesday, "Tue"},
      {DayOfWeek.Wednesday, "Wed"},
    };

    private static readonly Dictionary<DayOfWeek, DayOfWeek> dayCycle = new Dictionary<DayOfWeek, DayOfWeek>
    {
      {DayOfWeek.Friday, DayOfWeek.Saturday},
      {DayOfWeek.Monday, DayOfWeek.Tuesday},
      {DayOfWeek.Saturday, DayOfWeek.Sunday},
      {DayOfWeek.Sunday, DayOfWeek.Monday},
      {DayOfWeek.Thursday, DayOfWeek.Friday},
      {DayOfWeek.Tuesday, DayOfWeek.Wednesday},
      {DayOfWeek.Wednesday, DayOfWeek.Thursday},
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ToFileSafeString(this DateTime date)
    {
      return string.Format("{0:yyyy-MM-dd HH-mm-ss}", date);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public static DayOfWeek Next(this DayOfWeek day)
    {
      return dayCycle[day];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static string ToAbbreviatedString(this DayOfWeek dayOfWeek)
    {
      return dayAbbreviations[dayOfWeek];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int GetWeekRows(this DateTime date)
    {
      return date.GetWeekRows(DayOfWeek.Sunday);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <param name="firstDayOfWeek"></param>
    /// <returns></returns>
    public static int GetWeekRows(this DateTime date, DayOfWeek firstDayOfWeek)
    {
      var year = date.Year;
      var month = date.Month;

      var firstDayOfMonth = new DateTime(year, month, 1);
      DateTime lastDayOfMonth = firstDayOfMonth.LastDateOfMonth();

      var calendar = Thread.CurrentThread.CurrentCulture.Calendar;
      var lastWeek = calendar.GetWeekOfYear(lastDayOfMonth, CalendarWeekRule.FirstDay, firstDayOfWeek);
      var firstWeek = calendar.GetWeekOfYear(firstDayOfMonth, CalendarWeekRule.FirstDay, firstDayOfWeek);

      return lastWeek - firstWeek + 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static bool IsPayDay(this DateTime date)
    {
      return IsPayDay(date, firstPayDay);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static bool IsPayDay(this DateTime date, DateTime firstPayDay)
    {
      var ticks = (date - firstPayDay).Ticks;
      return (ticks % TwoWeeksTicks) == 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime FirstDateOfMonth(this DateTime date)
    {
      return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int LastDayOfMonth(this DateTime date)
    {
      return new DateTime(date.Year, date.Month, 1)
        .AddMonths(1)
        .AddDays(-1)
        .Day;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime LastDateOfMonth(this DateTime date)
    {
      return date.AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string GetMonthName(this DateTime date)
    {
      return CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(date.Month);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int Week(this DateTime date)
    {
      var remain = 0;
      var noOfWeek = 0;
      noOfWeek = Math.DivRem(date.Day, 7, out remain);
      if (remain > 0)
      {
        noOfWeek += 1;
      }
      return noOfWeek;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime FirstOfWeek(this DateTime date)
    {
      var sunday = (int)DayOfWeek.Sunday;
      var current = (int)date.DayOfWeek;
      var diff = current - sunday;
      return date.AddDays(-diff);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static bool IsWeekend(this DayOfWeek dayOfWeek)
    {
      return
        dayOfWeek == DayOfWeek.Sunday ||
          dayOfWeek == DayOfWeek.Saturday;
    }
  }
}