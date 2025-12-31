using FeedbackApi.Data;
using FeedbackApi.Models.DTOs;
using FeedbackApi.Models.Entities;
using FeedbackApi.Validators;
using Microsoft.EntityFrameworkCore;

namespace FeedbackApi.Services;

public class ReviewService(FeedbackDbContext context) : IReviewService
{
    public async Task<Result<ReviewResponse>> CreateAsync(CreateReviewRequest request, Guid userId)
    {
        var booking = await context.Bookings
            .Include(b => b.User)
            .Include(b => b.Apartment)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId);

        var existingReview = await context.Reviews
            .FirstOrDefaultAsync(r => r.BookingId == request.BookingId);

        var validationResult = ReviewValidator.ValidateCreate(request, booking, existingReview, userId);
        if (!validationResult.IsSuccess)
            return Result<ReviewResponse>.Failure(validationResult.Errors);

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ApartmentId = booking!.ApartmentId,
            BookingId = request.BookingId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        var response = MapToResponse(review, booking.User.Name, booking.Apartment.Address);
        return Result<ReviewResponse>.Success(response);
    }

    public async Task<Result<ReviewResponse>> UpdateAsync(Guid reviewId, UpdateReviewRequest request, Guid userId)
    {
        var review = await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Apartment)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        var validationResult = ReviewValidator.ValidateUpdate(request, review, userId);
        if (!validationResult.IsSuccess)
            return Result<ReviewResponse>.Failure(validationResult.Errors);

        review!.Rating = request.Rating;
        review.Comment = request.Comment;

        await context.SaveChangesAsync();

        var response = MapToResponse(review, review.User.Name, review.Apartment.Address);
        return Result<ReviewResponse>.Success(response);
    }

    public async Task<Result> DeleteAsync(Guid reviewId, Guid userId)
    {
        var review = await context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);

        var validationResult = ReviewValidator.ValidateDelete(review, userId);
        if (!validationResult.IsSuccess)
            return Result.Failure(validationResult.Errors);

        context.Reviews.Remove(review!);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<ReviewResponse>> GetByIdAsync(Guid reviewId)
    {
        var review = await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Apartment)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review is null)
            return Result<ReviewResponse>.Failure("Review not found");

        var response = MapToResponse(review, review.User.Name, review.Apartment.Address);
        return Result<ReviewResponse>.Success(response);
    }

    public async Task<Result<IEnumerable<ReviewResponse>>> GetByApartmentIdAsync(Guid apartmentId)
    {
        var apartmentExists = await context.Apartments.AnyAsync(a => a.Id == apartmentId);
        if (!apartmentExists)
            return Result<IEnumerable<ReviewResponse>>.Failure("Apartment not found");

        var reviews = await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Apartment)
            .Where(r => r.ApartmentId == apartmentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var responses = reviews.Select(r => MapToResponse(r, r.User.Name, r.Apartment.Address));
        return Result<IEnumerable<ReviewResponse>>.Success(responses);
    }

    private static ReviewResponse MapToResponse(Review review, string userName, string apartmentAddress) =>
        new(
            review.Id,
            review.UserId,
            userName,
            review.ApartmentId,
            apartmentAddress,
            review.BookingId,
            review.Rating,
            review.Comment,
            review.CreatedAt
        );
}

