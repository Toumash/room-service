using RoomService.Application.Date;
using RoomService.Application.Dto;

namespace RoomService.Application.Availability.Query;

public record SearchQuery(string HotelId, int LookAheadDays, string RoomType);

public record SearchQueryResponse(List<SearchResult> Results);

public record SearchResult(DateRange DataRange, int RoomsAvailable)
{
    public override string ToString()
    {
        return $"{RoomsAvailable}🛌 {DataRange}";
    }
};

public class SearchQueryHandler(
    List<HotelDto> hotels,
    List<BookingDto> bookings,
    IDateProvider dateProvider)
{
    public SearchQueryResponse Handle(SearchQuery query)
    {
        var today = dateProvider.GetToday();
        var end = today.AddDays(query.LookAheadDays);
        var dateRange = new DateRange(today, end);
        if (hotels.All(h => h.id != query.HotelId))
        {
            throw new ArgumentOutOfRangeException(nameof(query.HotelId), "hotel does not exist in the database");
        }

        var roomsForRequestedType = hotels
            .First(h => h.id == query.HotelId)
            .rooms
            .Where(r => r.roomType == query.RoomType)
            .ToList();

        if (!roomsForRequestedType.Any())
        {
            throw new ArgumentOutOfRangeException(nameof(query.RoomType), "roomtype does not exist in the database");
        }

        var availabilityStore = RoomAvailability.BuildAvailability(hotels, bookings);
        var results = new List<SearchResult>();

        DateOnly? periodStart = null;
        DateOnly? periodEnd = null;
        int lastAvailability = 0;

        foreach (var date in dateRange)
        {
            var availableRooms = roomsForRequestedType.Sum(room =>
                availabilityStore.AvailabilityFor(query.HotelId, room.roomId, date) > 0 ? 1 : 0);

            if (availableRooms > 0)
            {
                if (periodStart == null)
                {
                    periodStart = date;
                    lastAvailability = availableRooms;
                }
                else
                {
                    if (lastAvailability != availableRooms)
                    {
                        results.Add(new SearchResult(new DateRange(periodStart.Value, periodEnd.Value),
                            lastAvailability));
                        periodStart = date;
                        periodEnd = null;
                        lastAvailability = availableRooms;
                    }
                }

                periodEnd = date;
            }
            else
            {
                if (periodStart != null && periodEnd != null)
                {
                    results.Add(new SearchResult(new DateRange(periodStart.Value, periodEnd.Value),
                        lastAvailability));
                    periodStart = null;
                    periodEnd = null;
                    lastAvailability = int.MaxValue;
                }
            }
        }

        if (periodStart != null && periodEnd != null)
        {
            results.Add(new SearchResult(new DateRange(periodStart.Value, periodEnd.Value),
                lastAvailability));
        }

        return new SearchQueryResponse(results);
    }
}