using RoomService.Application.Dto;

namespace RoomService.Application.Availability;

public class RoomAvailability
{
    private readonly Dictionary<string, Dictionary<string, Dictionary<DateOnly, int>>> store;
    private const int DefaultAvailable = 1;

    public RoomAvailability(Dictionary<string, Dictionary<string, Dictionary<DateOnly, int>>> prefilled)
    {
        store = prefilled;
    }

    /// <summary>
    /// dictionary of hotel -> roomId -> data -> availability (1, 0 used, -1 overbooked)
    /// </summary>
    /// <param name="hotels"></param>
    /// <param name="bookings"></param>
    /// <returns></returns>
    // hidden requirement: do we want to move customer from one room to another? I guess not
    public static RoomAvailability BuildAvailability(
        List<HotelDto> hotels, List<BookingDto> bookings)
    {
        var store = new Dictionary<string, Dictionary<string, Dictionary<DateOnly, int>>>();
        foreach (var hotel in hotels)
        {
            var singleHotelRoomsAvailability = new Dictionary<string, Dictionary<DateOnly, int>>();
            foreach (var room in hotel.rooms)
            {
                singleHotelRoomsAvailability[room.roomId] = new Dictionary<DateOnly, int>();
            }

            store[hotel.id] = singleHotelRoomsAvailability;
        }

        var ra = new RoomAvailability(store);

        foreach (var b in bookings)
        {
            var roomIdsForThisType = hotels.First(h => h.id == b.hotelId).rooms.Where(r => r.roomType == b.roomType)
                .ToList();
            var dateRange = new DateRange(b.arrival, b.departure);

            var rooms = roomIdsForThisType.Select(room => new
            {
                MinAvailability =
                    dateRange.AsEnumerable()
                        .Min(date => ra.AvailabilityFor(b.hotelId, room.roomId, date)),
                Days = dateRange.AsEnumerable().Count(),
                Room = room
            }).ToList();

            var selectedRoomId = rooms.MaxBy(r => r.MinAvailability).Room.roomId;

            foreach (var date in dateRange.AsEnumerable())
            {
                ra.MarkSlotAsTaken(b.hotelId, selectedRoomId, date);
            }
        }

        return new RoomAvailability(store);
    }

    public void MarkSlotAsTaken(string hotelId, string roomId, DateOnly date)
    {
        if (store[hotelId][roomId].ContainsKey(date))
        {
            store[hotelId][roomId][date] = store[hotelId][roomId][date] - 1;
        }
        else
        {
            store[hotelId][roomId][date] = DefaultAvailable - 1;
        }
    }

    public int AvailabilityFor(string hotelId, string roomId, DateOnly date)
    {
        return store[hotelId][roomId].ContainsKey(date) ? store[hotelId][roomId][date] : DefaultAvailable;
    }

    public void Print()
    {
        var datesToShow = store.SelectMany(s => s.Value.SelectMany(x => x.Value).Select(x => x.Key)).Distinct().Order()
            .ToList();
        Console.WriteLine($"{"dates",7}{string.Join("", datesToShow.Select(d => d.Day))}");
        foreach ((var hotelId, var rooms) in store)
        {
            Console.WriteLine($"--- HOTEL {hotelId} ---");
            foreach (var (roomId, dates) in rooms)
            {
                Console.Write($"r:{roomId,5} |");
                foreach (var date in datesToShow)
                {
                    Console.Write(AvailabilityFor(hotelId, roomId, date));
                }

                Console.WriteLine();
            }
        }

        Console.WriteLine("Legend: 1 = Free, 0 = Taken, -1 = Overbooked");
    }
}