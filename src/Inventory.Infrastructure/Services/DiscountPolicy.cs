using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.Infrastructure.Services;

public class DiscountPolicy : IDiscountPolicy
{
    private static List<DateOnly>? _bankHolidays = null;

    private static readonly List<(int month, int day)> _staticBankHolidays =
    [
        (1, 1),
        (1, 6),
        (5, 1),
        (5, 3),
        (8, 15),
        (11, 1),
        (11, 11),
        (12, 25),
        (12, 26)
    ];

    private static DateOnly GetEasterDate(int year)
    {
        var a = year % 19;
        var b = year / 100;
        var c = year % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var month = (h + l - 7 * m + 114) / 31;
        var day = ((h + l - 7 * m + 114) % 31) + 1;

        return new DateOnly(year, month, day);
    }

    private static List<DateOnly> PrepareBankHolidays(int year)
    {
        var easter = GetEasterDate(year);
        var result = new List<DateOnly>();
        result.AddRange(_staticBankHolidays.Select(s => new DateOnly(year, s.month, s.day)));
        result.Add(easter);
        result.Add(easter.AddDays(1));
        result.Add(easter.AddDays(60));
        return result;
    }

    private static DateOnly GetBlackFriday(int year)
    {
        //Calculate the date of the Black Friday (fourth Friday in November)
        var date = new DateOnly(year, 11, 1);
        int daysToFriday = ((int)DayOfWeek.Friday - (int)date.DayOfWeek + 7) % 7;
        var firstFriday = date.AddDays(daysToFriday);
        return firstFriday.AddDays(21);
    }

    public decimal CalculateDiscount(int position, Product product, decimal unitPrice, int quantity, DateTime date)
    {
        _bankHolidays ??= PrepareBankHolidays(date.Year);
        var listOfDiscounts = new List<decimal>()
        {
            0
        };
        var dateAsDateOnly = new DateOnly(date.Year, date.Month, date.Day);
        if (GetBlackFriday(date.Year) == dateAsDateOnly)
            listOfDiscounts.Add(25);
        if (position == 0 && _bankHolidays.Contains(dateAsDateOnly))
            listOfDiscounts.Add(15);
        switch (quantity)
        {
            case >= 5 and < 10:
                listOfDiscounts.Add(10);
                break;
            case >= 10 and < 50:
                listOfDiscounts.Add(20);
                break;
            case >= 50:
                listOfDiscounts.Add(30);
                break;
        }

        var discount = listOfDiscounts.OrderDescending().First();
        return unitPrice * ((100 - discount) / (decimal)100.0);
    }
}