using FeedbackApi.Models.DTOs;
using FeedbackApi.Models.Entities;
using FeedbackApi.Models.Enums;
using FeedbackApi.Services;

namespace FeedbackApi.Validators;

public class ReviewValidator
{
    public static Result ValidateCreate(CreateReviewRequest request, Booking? booking, Review? existingReview, Guid userId)
    {
        var errors = new List<string>();

        // Rating validation
        if (request.Rating is < 1 or > 5)
            errors.Add("Rating must be between 1 and 5");

        // Comment validation
        if (string.IsNullOrWhiteSpace(request.Comment))
            errors.Add("Comment is required");
        else if (request.Comment.Length > 2000)
            errors.Add("Comment must not exceed 2000 characters");

        // Booking validation
        if (booking is null)
        {
            errors.Add("Booking not found");
        }
        else
        {
            // Check if booking belongs to user
            if (booking.UserId != userId)
                errors.Add("You can only review your own bookings");

            // Check if booking is completed
            if (booking.Status != BookingStatus.CheckedOut)
                errors.Add("You can only review completed stays");

            // Check if checkout date has passed
            if (booking.CheckOutDate > DateTime.UtcNow)
                errors.Add("You can only review after checkout");
        }

        // Duplicate review check
        if (existingReview is not null)
            errors.Add("You have already reviewed this booking");

        return errors.Count > 0
            ? Result.Failure(errors)
            : Result.Success();
    }

    public static Result ValidateUpdate(UpdateReviewRequest request, Review? review, Guid userId)
    {
        var errors = new List<string>();

        // Review existence check
        if (review is null)
        {
            errors.Add("Review not found");
            return Result.Failure(errors);
        }

        // Ownership check
        if (review.UserId != userId)
            errors.Add("You can only edit your own reviews");

        // Rating validation
        if (request.Rating is < 1 or > 5)
            errors.Add("Rating must be between 1 and 5");

        // Comment validation
        if (string.IsNullOrWhiteSpace(request.Comment))
            errors.Add("Comment is required");
        else if (request.Comment.Length > 2000)
            errors.Add("Comment must not exceed 2000 characters");

        return errors.Count > 0
            ? Result.Failure(errors)
            : Result.Success();
    }

    public static Result ValidateDelete(Review? review, Guid userId)
    {
        var errors = new List<string>();

        // Review existence check
        if (review is null)
        {
            errors.Add("Review not found");
            return Result.Failure(errors);
        }

        // Ownership check
        if (review.UserId != userId)
            errors.Add("You can only delete your own reviews");

        return errors.Count > 0
            ? Result.Failure(errors)
            : Result.Success();
    }
}

