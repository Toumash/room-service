using RoomService.Application.Date;
using RoomService.Application.Dto;

namespace RoomService.Application.Availability.Query;

public class SearchQuerySafeStrategyHandler
{
    public SearchQueryResponse Handle(SearchQuery query, List<HotelDto> hotels,
        List<BookingDto> bookings, IDateProvider dateProvider)
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
        int minAvailability = int.MaxValue;
        
        // prompt: given AvailabilityFor(hotelId, roomId, date) function write a search function body that will search
        // periods of free slots. When the function returns > 0 that means the room is available. Write it in C#. Iterate over dateRange of `DateOnly`. Return a list of DateRange record DateRange(DateOnly Start, DateOnly End)
        foreach (var date in dateRange)
        {
            var availableRooms = roomsForRequestedType.Sum(room =>
                availabilityStore.AvailabilityFor(query.HotelId, room.roomId, date) > 0 ? 1 : 0);

            if (availableRooms > 0)
            {
                if (periodStart == null)
                {
                    periodStart = date;
                    minAvailability = availableRooms;
                }
                else
                {
                    minAvailability = Math.Min(minAvailability, availableRooms);
                }

                periodEnd = date;
            }
            else
            {
                if (periodStart != null && periodEnd != null)
                {
                    results.Add(new SearchResult(new DateRange(periodStart.Value, periodEnd.Value),
                        minAvailability));
                    periodStart = null;
                    periodEnd = null;
                    minAvailability = int.MaxValue;
                }
            }
        }

        if (periodStart != null && periodEnd != null)
        {
            results.Add(new SearchResult(new DateRange(periodStart.Value, periodEnd.Value),
                minAvailability));
        }

        return new SearchQueryResponse(results);
    }
}