using RoomService.Application.Availability.Query;
using RoomService.Application.Dto;
using Shouldly;

namespace RoomService.Tests.Availability;

public class HotelAvailabilityTests
{
    [InlineData("H1", "20240901", "SGL", 1)]
    [InlineData("H1", "20240901-20240903", "DBL", 1)]
    [Theory]
    public void WhenNoReservations_ThenShouldTreatRoomAsFree(string hotelId, string dateRange, string roomType,
        int expectedCount)
    {
        var (hotels, bookings) = GetOriginalTestData();

        var response = new CheckAvailabilityQueryHandler(hotels, bookings)
            .Handle(new CheckAvailabilityQuery(hotelId, dateRange, roomType));

        response.ShouldBe(new CheckAvailabilityQueryResponse(expectedCount));
    }

    [InlineData("H1", "20240902-20240903", "DBL", 0)]
    [InlineData("H1", "20240902-20240903", "SGL", 0)]
    [InlineData("H1", "20240901-20240902", "SGL", 1)]
    [InlineData("H1", "20240903-20240904", "DBL", 1)]
    [Theory]
    public void WhenOneRoomIsFreeAlways_ButReservationWouldForceACustomerToSwitchRooms_ShouldNotMarkItAsFree(
        string hotelId, string dateRange,
        string roomType, int expectedCount)
    {
        var (hotels, bookings) = GetOneRoomIsAlwaysFree_ButEverydayADifferentOne();

        var response = new CheckAvailabilityQueryHandler(hotels, bookings)
            .Handle(new CheckAvailabilityQuery(hotelId, dateRange, roomType));

        response.ShouldBe(new CheckAvailabilityQueryResponse(expectedCount));
    }


    [InlineData("H1", "20240902-20240903", "DBL", -2)]
    [Theory]
    public void WhenRoomIsOverbooked_ShouldReturnNegativeAvailability(
        string hotelId, string dateRange,
        string roomType, int expectedCount)
    {
        var (hotels, bookings) = GetOverbookingExample();

        var response = new CheckAvailabilityQueryHandler(hotels, bookings)
            .Handle(new CheckAvailabilityQuery(hotelId, dateRange, roomType));

        response.ShouldBe(new CheckAvailabilityQueryResponse(expectedCount));
    }


    [InlineData("H1", "20240902-20240903", "DBL", 0)]
    [Theory]
    public void WhenOverbooked_ShouldProperlyAssignRooms(string hotelId, string dateRange, string roomType,
        int expectedCount)
    {
        var (hotels, bookings) = GetOverbookingExample2();

        var response = new CheckAvailabilityQueryHandler(hotels, bookings)
            .Handle(new CheckAvailabilityQuery(hotelId, dateRange, roomType));

        response.ShouldBe(new CheckAvailabilityQueryResponse(expectedCount));
    }

    private (List<HotelDto> hotels, List<BookingDto> bookings) GetOverbookingExample2()
    {
        Room[] rooms =
        [
            new Room("DBL", "2"),
            new Room("DBL", "3"),
        ];
        return (
            [
                new HotelDto(
                    "H1",
                    default,
                    rooms.Select(r => new RoomType(r.roomType, default, default, default)).ToArray(),
                    rooms.ToArray())
            ],
            [
                new BookingDto("H1", new DateOnly(2024, 09, 02), new DateOnly(2024, 09, 02), "DBL", default),
                new BookingDto("H1", new DateOnly(2024, 09, 03), new DateOnly(2024, 09, 05), "DBL", default),
                new BookingDto("H1", new DateOnly(2024, 09, 02), new DateOnly(2024, 09, 03), "DBL", default),
                new BookingDto("H1", new DateOnly(2024, 09, 02), new DateOnly(2024, 09, 04), "DBL", default),
            ]);
    }

    private (List<HotelDto> hotels, List<BookingDto> bookings) GetOverbookingExample()
    {
        Room[] rooms =
        [
            new Room("DBL", "3"),
        ];
        return (
            [
                new HotelDto(
                    "H1",
                    default,
                    rooms.Select(r => new RoomType(r.roomType, default, default, default)).ToArray(),
                    rooms.ToArray())
            ],
            [
                new BookingDto("H1", new DateOnly(2024, 09, 02), new DateOnly(2024, 09, 02), "DBL", default),
                new BookingDto("H1", new DateOnly(2024, 09, 02), new DateOnly(2024, 09, 02), "DBL", default),
                new BookingDto("H1", new DateOnly(2024, 09, 02), new DateOnly(2024, 09, 02), "DBL", default),
            ]);
    }


    private (List<HotelDto> hotels, List<BookingDto> bookings) GetOneRoomIsAlwaysFree_ButEverydayADifferentOne()
    {
        Room[] rooms =
        [
            new Room("SGL", "1"),
            new Room("DBL", "3"),
        ];
        return (
            [
                new HotelDto(
                    "H1",
                    default,
                    rooms.Select(r => new RoomType(r.roomType, default, default, default)).ToArray(),
                    rooms.ToArray())
            ],
            [
                new BookingDto("H1", new DateOnly(2024, 09, 02), new DateOnly(2024, 09, 02), "DBL", default),
                new BookingDto("H1", new DateOnly(2024, 09, 03), new DateOnly(2024, 09, 03), "SGL", default),
            ]);
    }

    private (List<HotelDto> hotels, List<BookingDto> bookings) GetOriginalTestData()
    {
        Room[] rooms =
        [
            new Room("SGL", "1"),
            new Room("SGL", "2"),
            new Room("DBL", "3"),
            new Room("DBL", "4"),
        ];
        return (
            [
                new HotelDto(
                    "H1",
                    default,
                    rooms.Select(r => new RoomType(r.roomType, default, default, default)).ToArray(),
                    rooms.ToArray())
            ],
            [
                new BookingDto("H1", new DateOnly(2024, 09, 01), new DateOnly(2024, 09, 03), "DBL", default),
                new BookingDto("H1", new DateOnly(2024, 09, 02), new DateOnly(2024, 09, 05), "SGL", default),
            ]);
    }
}