namespace RoomService.Application.Dto;

public record HotelDto(
    string id,
    string name,
    RoomType[] roomTypes,
    Room[] rooms
);

public record RoomType(
    string code,
    string description,
    string[] amenities,
    string[] features
);

public record Room(
    string roomType,
    string roomId
);