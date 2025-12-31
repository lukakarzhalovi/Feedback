namespace FeedbackApi.Models.DTOs;

public record UpdateReviewRequest(
    int Rating,
    string Comment
);

