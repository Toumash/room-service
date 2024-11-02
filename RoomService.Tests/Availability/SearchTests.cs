using RoomService.Application;
using RoomService.Application.Availability.Query;
using RoomService.Application.Date;
using RoomService.Application.Dto;
using Shouldly;

namespace RoomService.Tests.Availability;

public class SearchTests
{
    [Fact]
    public void ShouldOfferWholeYearAsFreeIfPossible()
    {
        var (hotels, bookings) = GetOriginalTestData();

        var response =
            new SearchQueryHandler(hotels, bookings, new FakeDateProvider(new DateOnly(2024, 11, 02)))
                .Handle(new SearchQuery("H1", 365, "SGL"));

        response.Results.ShouldBe(
            [new SearchResult(new DateRange(new DateOnly(2024, 11, 02), new DateOnly(2025, 11, 02)), 2)]);
    }

    [Fact]
    public void ShouldWorkWithBasicExample()
    {
        var (hotels, bookings) = GetOriginalTestData();

        var response =
            new SearchQueryHandler(hotels, bookings, new FakeDateProvider(new DateOnly(2024, 11, 02))).Handle(
                new SearchQuery("H1", 365, "SGL"));

        response.Results.ShouldBe(
            [new SearchResult(new DateRange(new DateOnly(2024, 11, 02), new DateOnly(2025, 11, 02)), 2)]);
    }

    [Fact]
    public void ShouldWorkWithBasicExample2()
    {
        var (hotels, bookings) = GetTestData();

        var response = new SearchQueryHandler(hotels,
            bookings, new FakeDateProvider(new DateOnly(2024, 11, 01))).Handle(new SearchQuery("H1", 365, "SGL"));

        response.Results.ShouldBe(
        [
            new SearchResult(new DateRange(new DateOnly(2024, 11, 01), new DateOnly(2024, 11, 01)), 2),
            new SearchResult(new DateRange(new DateOnly(2024, 11, 02), new DateOnly(2024, 11, 03)), 1),
            new SearchResult(new DateRange(new DateOnly(2024, 11, 04), new DateOnly(2025, 11, 01)), 2),
        ]);
    }

    [Fact]
    public void WhenOverbooked_ShouldNotCrash()
    {
        var (hotels, bookings) = GetOverbookedTestData();

        var response =
            new SearchQueryHandler(hotels, bookings, new FakeDateProvider(new DateOnly(2024, 11, 01)))
                .Handle(new SearchQuery("H1", 365, "SGL"));

        response.Results.ShouldBe(
        [
            new SearchResult(new DateRange(new DateOnly(2024, 11, 01), new DateOnly(2024, 11, 01)), 2),
            new SearchResult(new DateRange(new DateOnly(2024, 11, 04), new DateOnly(2024, 11, 04)), 1),
            new SearchResult(new DateRange(new DateOnly(2024, 11, 05), new DateOnly(2025, 11, 01)), 2),
        ]);
    }

    public (List<HotelDto> hotels, List<BookingDto> bookings) GetOverbookedTestData()
    {
        Room[] rooms =
        [
            new Room("SGL", "1"),
            new Room("SGL", "2"),
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
                new BookingDto("H1", new DateOnly(2024, 11, 02), new DateOnly(2024, 11, 03), "SGL", default),
                new BookingDto("H1", new DateOnly(2024, 11, 02), new DateOnly(2024, 11, 03), "SGL", default),
                new BookingDto("H1", new DateOnly(2024, 11, 02), new DateOnly(2024, 11, 04), "SGL", default),
            ]);
    }

    public (List<HotelDto> hotels, List<BookingDto> bookings) GetTestData()
    {
        Room[] rooms =
        [
            new Room("SGL", "1"),
            new Room("SGL", "2"),
            new Room("SGB", "3"),
        ];
        return (
            [
                new HotelDto(
                    "H1",
                    default,
                    rooms.Select(r => new RoomType(r.roomType, default, default, default)).ToArray(),
                    rooms.ToArray())
            ],
            [new BookingDto("H1", new DateOnly(2024, 11, 02), new DateOnly(2024, 11, 03), "SGL", default)]);
    }

    public (List<HotelDto> hotels, List<BookingDto> bookings) GetOriginalTestData()
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