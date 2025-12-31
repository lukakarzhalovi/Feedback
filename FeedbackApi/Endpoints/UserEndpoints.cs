using FeedbackApi.Data;
using FeedbackApi.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FeedbackApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users");

        group.MapGet("", GetAll)
            .WithName("GetAllUsers")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetUserById")
            .WithOpenApi();

        group.MapGet("/{id:guid}/bookings", GetUserBookings)
            .WithName("GetUserBookings")
            .WithOpenApi();

        group.MapGet("/{id:guid}/reviews", GetUserReviews)
            .WithName("GetUserReviews")
            .WithOpenApi();
    }

    private static async Task<IResult> GetAll(FeedbackDbContext context)
    {
        var users = await context.Users
            .Select(u => new
            {
                u.Id,
                u.Name,
                BookingCount = u.Bookings.Count,
                ReviewCount = u.Reviews.Count
            })
            .ToListAsync();

        return Results.Ok(users);
    }

    private static async Task<IResult> GetById(Guid id, FeedbackDbContext context)
    {
        var user = await context.Users
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.Name,
                BookingCount = u.Bookings.Count,
                ReviewCount = u.Reviews.Count
            })
            .FirstOrDefaultAsync();

        return user is null
            ? Results.NotFound(new { Error = "User not found" })
            : Results.Ok(user);
    }

    private static async Task<IResult> GetUserBookings(Guid id, FeedbackDbContext context)
    {
        var userExists = await context.Users.AnyAsync(u => u.Id == id);
        if (!userExists)
            return Results.NotFound(new { Error = "User not found" });

        var bookings = await context.Bookings
            .Where(b => b.UserId == id)
            .Include(b => b.Apartment)
            .Select(b => new
            {
                b.Id,
                b.ApartmentId,
                ApartmentAddress = b.Apartment.Address,
                b.CheckInDate,
                b.CheckOutDate,
                Status = b.Status.ToString(),
                HasReview = b.Review != null
            })
            .ToListAsync();

        return Results.Ok(bookings);
    }

    private static async Task<IResult> GetUserReviews(Guid id, FeedbackDbContext context)
    {
        var userExists = await context.Users.AnyAsync(u => u.Id == id);
        if (!userExists)
            return Results.NotFound(new { Error = "User not found" });

        var reviews = await context.Reviews
            .Where(r => r.UserId == id)
            .Include(r => r.Apartment)
            .Select(r => new
            {
                r.Id,
                r.ApartmentId,
                ApartmentAddress = r.Apartment.Address,
                r.BookingId,
                r.Rating,
                r.Comment,
                r.CreatedAt
            })
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Results.Ok(reviews);
    }
}

