namespace RoomService.Application.Dto;

public record BookingDto(
    string hotelId,
    DateOnly arrival,
    DateOnly departure,
    string roomType,
    string roomRate
);