namespace RoomService.Application.Date;

public interface IDateProvider
{
    public DateOnly GetToday();
}

public class DateProvider : IDateProvider
{
    public DateOnly GetToday() => DateOnly.FromDateTime(DateTime.Today);
}