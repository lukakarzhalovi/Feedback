using FeedbackApi.Data;
using FeedbackApi.Services;
using Microsoft.EntityFrameworkCore;

namespace FeedbackApi.Endpoints;

public static class ApartmentEndpoints
{
    public static void MapApartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/apartments")
            .WithTags("Apartments");

        group.MapGet("", GetAll)
            .WithName("GetAllApartments")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetApartmentById")
            .WithOpenApi();

        group.MapGet("/{apartmentId:guid}/reviews", GetReviews)
            .WithName("GetApartmentReviews")
            .WithOpenApi();
    }

    private static async Task<IResult> GetAll(FeedbackDbContext context)
    {
        var apartments = await context.Apartments
            .Select(a => new
            {
                a.Id,
                a.Address,
                ReviewCount = a.Reviews.Count,
                AverageRating = a.Reviews.Count != 0 ? a.Reviews.Average(r => r.Rating) : 0
            })
            .ToListAsync();

        return Results.Ok(apartments);
    }

    private static async Task<IResult> GetById(Guid id, FeedbackDbContext context)
    {
        var apartment = await context.Apartments
            .Where(a => a.Id == id)
            .Select(a => new
            {
                a.Id,
                a.Address,
                ReviewCount = a.Reviews.Count,
                AverageRating = a.Reviews.Count != 0 ? a.Reviews.Average(r => r.Rating) : 0
            })
            .FirstOrDefaultAsync();

        return apartment is null
            ? Results.NotFound(new { Error = "Apartment not found" })
            : Results.Ok(apartment);
    }

    private static async Task<IResult> GetReviews(Guid apartmentId, IReviewService reviewService)
    {
        var result = await reviewService.GetByApartmentIdAsync(apartmentId);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new { result.Errors });
    }
}

