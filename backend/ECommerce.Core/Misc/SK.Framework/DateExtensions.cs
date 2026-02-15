using System;

namespace SK.Framework;

public static class DateExtensions
{
    public static DateTime EndOfDay(this DateTime self) => new DateTime(self.Year, self.Month, self.Day, 23, 59, 59);

    public static DateTime StartOfDay(this DateTime self) => new DateTime(self.Year, self.Month, self.Day, 0, 0, 0);
}
