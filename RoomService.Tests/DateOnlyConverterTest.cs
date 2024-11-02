using System.Text.Json;
using RoomService.Application;
using Shouldly;

namespace RoomService.Tests;

public class DateOnlyConverterTest
{
    [Fact]
    public void ShouldConvertDateOnlyJsonProperly()
    {
        var json = "{ \"arrival\": \"20240901\" }";
        var options = new JsonSerializerOptions
        {
            Converters = { new DateOnlyConverter() }
        };
        var obj = JsonSerializer.Deserialize<TestJson>(json, options)!;
        obj.arrival.ShouldBe(new DateOnly(2024, 09, 01));
    }

    class TestJson
    {
        public DateOnly arrival { get; set; }
    }
}