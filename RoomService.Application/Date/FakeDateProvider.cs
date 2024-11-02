namespace RoomService.Application.Date;

public class FakeDateProvider(DateOnly today) : IDateProvider
{
    public DateOnly GetToday() => today;
}