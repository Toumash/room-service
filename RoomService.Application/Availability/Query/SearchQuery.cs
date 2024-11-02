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