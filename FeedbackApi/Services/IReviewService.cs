using FeedbackApi.Models.DTOs;

namespace FeedbackApi.Services;

public interface IReviewService
{
    Task<Result<ReviewResponse>> CreateAsync(CreateReviewRequest request, Guid userId);
    Task<Result<ReviewResponse>> UpdateAsync(Guid reviewId, UpdateReviewRequest request, Guid userId);
    Task<Result> DeleteAsync(Guid reviewId, Guid userId);
    Task<Result<ReviewResponse>> GetByIdAsync(Guid reviewId);
    Task<Result<IEnumerable<ReviewResponse>>> GetByApartmentIdAsync(Guid apartmentId);
}

