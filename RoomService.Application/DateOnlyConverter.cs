namespace RoomService.Application;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

// prompt: write a custom System.Text.Json date converter for reading yyyyMMdd date from string to DateOnly in .net
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyyMMdd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string token type.");
        }

        string dateString = reader.GetString();

        if (DateOnly.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateOnly date))
        {
            return date;
        }
        else
        {
            throw new JsonException($"Unable to parse '{dateString}' to a DateOnly object.");
        }
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}