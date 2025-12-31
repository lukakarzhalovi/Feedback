namespace FeedbackApi.Models.DTOs;

public record ReviewResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid ApartmentId,
    string ApartmentAddress,
    Guid BookingId,
    int Rating,
    string Comment,
    DateTime CreatedAt
);

