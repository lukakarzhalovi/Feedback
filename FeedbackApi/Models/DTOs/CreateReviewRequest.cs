namespace FeedbackApi.Models.DTOs;

public record CreateReviewRequest(
    Guid BookingId,
    int Rating,
    string Comment
);

