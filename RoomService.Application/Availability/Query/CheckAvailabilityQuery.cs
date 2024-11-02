using RoomService.Application.Dto;

namespace RoomService.Application.Availability.Query;

public record CheckAvailabilityQuery(string HotelId, string DataRange, string RoomType);

public record CheckAvailabilityQueryResponse(int AvailableCount);

public class CheckAvailabilityQueryHandler(
    List<HotelDto> hotels,
    List<BookingDto> bookings)
{
    public CheckAvailabilityQueryResponse Handle(CheckAvailabilityQuery query)
    {
        var dateRange = DateRange.ValueOf(query.DataRange);
        if (hotels.All(h => h.id != query.HotelId))
        {
            throw new ArgumentOutOfRangeException(nameof(query.HotelId), "hotel does not exist in the database");
        }

        var roomsForRequestedType =
            hotels.First(h => h.id == query.HotelId).rooms.Where(r => r.roomType == query.RoomType).ToList();
        if (!roomsForRequestedType.Any())
        {
            throw new ArgumentOutOfRangeException(nameof(query.RoomType), "roomtype does not exist in the database");
        }

        var availability = RoomAvailability.BuildAvailability(hotels, bookings);

        var rooms = roomsForRequestedType.Select(room => new
        {
            MinAvailability =
                dateRange.AsEnumerable()
                    .Min(date => availability.AvailabilityFor(query.HotelId, room.roomId, date)),
            Days = dateRange.AsEnumerable().Count(),
            Room = room
        }).ToList();

        return new CheckAvailabilityQueryResponse(rooms.MaxBy(r => r.MinAvailability).MinAvailability);
    }
}