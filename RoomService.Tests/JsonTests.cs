using System.Text.Json;
using RoomService.Application;
using RoomService.Application.Dto;
using Shouldly;

namespace RoomService.Tests;

public class JsonTests
{
    [Fact]
    public void Bookings_ShouldNotThrow()
    {
        var a = () => GetTestBookings();
        a.ShouldNotThrow();
    }

    [Fact]
    public void Hotels_ShouldNotThrow()
    {
        var a = () => GetTestHotels();
        a.ShouldNotThrow();
    }


    public List<HotelDto> GetTestHotels()
    {
        var hotelsStr =
            "[\n  {\n    \"id\": \"H1\",\n    \"name\": \"Hotel California\",\n    \"roomTypes\": [\n      {\n        \"code\": \"SGL\",\n        \"description\": \"Single Room\",\n        \"amenities\": [\"WiFi\", \"TV\"],\n        \"features\": [\"Non-smoking\"]\n      },\n      {\n        \"code\": \"DBL\",\n        \"description\": \"Double Room\",\n        \"amenities\": [\"WiFi\", \"TV\", \"Minibar\"],\n        \"features\": [\"Non-smoking\", \"Sea View\"]\n      }\n    ],\n    \"rooms\": [\n      {\n        \"roomType\": \"SGL\",\n        \"roomId\": \"101\"\n      },\n      {\n        \"roomType\": \"SGL\",\n        \"roomId\": \"102\"\n      },\n      {\n        \"roomType\": \"DBL\",\n        \"roomId\": \"201\"\n      },\n      {\n        \"roomType\": \"DBL\",\n        \"roomId\": \"202\"\n      }\n    ]\n  }\n]";
        var options = new JsonSerializerOptions
        {
            Converters = { new DateOnlyConverter() }
        };
        var hotels = JsonSerializer.Deserialize<List<HotelDto>>(hotelsStr, options)!;
        return hotels;
    }

    public List<BookingDto> GetTestBookings()
    {
        var bookingsStr =
            "[\n  {\n    \"hotelId\": \"H1\",\n    \"arrival\": \"20240901\",\n    \"departure\": \"20240903\",\n    \"roomType\": \"DBL\",\n    \"roomRate\": \"Prepaid\"\n  },\n  {\n    \"hotelId\": \"H1\",\n    \"arrival\": \"20240902\",\n    \"departure\": \"20240905\",\n    \"roomType\": \"SGL\",\n    \"roomRate\": \"Standard\"\n  }\n]";
        var options = new JsonSerializerOptions
        {
            Converters = { new DateOnlyConverter() }
        };
        var bookings = JsonSerializer.Deserialize<List<BookingDto>>(bookingsStr, options)!;
        return bookings;
    }
}