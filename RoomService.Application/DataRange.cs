using System.Collections;

namespace RoomService.Application;

public record DateRange(DateOnly Start, DateOnly End) : IEnumerable<DateOnly>
{
    public static DateRange ValueOf(string str)
    {
        if (str.Contains("-"))
        {
            var d = str.Split("-");
            var start = d[0];
            var end = d[1];
            return new DateRange(DateOnly.ParseExact(start, "yyyyMMdd"), DateOnly.ParseExact(end, "yyyyMMdd"));
        }
        else
        {
            var startDate = DateOnly.ParseExact(str, "yyyyMMdd");
            return new DateRange(startDate, startDate);
        }
    }

    public IEnumerator<DateOnly> GetEnumerator()
    {
        for (var dt = Start; dt <= End; dt = dt.AddDays(1))
        {
            yield return dt;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        if (Start != End)
            return $"{Start}-{End}";
        else
        {
            return $"{Start}";
        }
    }
}