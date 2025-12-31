using FeedbackApi.Models.DTOs;
using FeedbackApi.Services;

namespace FeedbackApi.Endpoints;

public static class ReviewEndpoints
{
    public static void MapReviewEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reviews")
            .WithTags("Reviews");

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetReviewById")
            .WithOpenApi();

        group.MapPost("", Create)
            .WithName("CreateReview")
            .WithOpenApi();

        group.MapPut("/{id:guid}", Update)
            .WithName("UpdateReview")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", Delete)
            .WithName("DeleteReview")
            .WithOpenApi();
    }

    private static async Task<IResult> GetById(Guid id, IReviewService reviewService)
    {
        var result = await reviewService.GetByIdAsync(id);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new { result.Errors });
    }

    private const string UserIdError = "X-User-Id header is required";


    private static async Task<IResult> Create(
        CreateReviewRequest request,
        IReviewService reviewService,
        HttpContext httpContext)
    {
        var userId = GetUserIdFromHeader(httpContext);
        if (userId is null)
            return Results.BadRequest(new { Errors = UserIdError });

        var result = await reviewService.CreateAsync(request, userId.Value);

        return result.IsSuccess
            ? Results.Created($"/api/reviews/{result.Value!.Id}", result.Value)
            : Results.BadRequest(new { result.Errors });
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateReviewRequest request,
        IReviewService reviewService,
        HttpContext httpContext)
    {
        var userId = GetUserIdFromHeader(httpContext);
        if (userId is null)
            return Results.BadRequest(new { Errors = UserIdError });

        var result = await reviewService.UpdateAsync(id, request, userId.Value);

        if (!result.IsSuccess)
        {
            return result.Errors.Contains("Review not found")
                ? Results.NotFound(new { result.Errors })
                : Results.BadRequest(new { result.Errors });
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> Delete(
        Guid id,
        IReviewService reviewService,
        HttpContext httpContext)
    {
        var userId = GetUserIdFromHeader(httpContext);
        if (userId is null)
            return Results.BadRequest(new { Errors = UserIdError });

        var result = await reviewService.DeleteAsync(id, userId.Value);

        if (!result.IsSuccess)
        {
            return result.Errors.Contains("Review not found")
                ? Results.NotFound(new { result.Errors })
                : Results.BadRequest(new { result.Errors });
        }

        return Results.NoContent();
    }

    private static Guid? GetUserIdFromHeader(HttpContext httpContext)
    {
        var userIdHeader = httpContext.Request.Headers["X-User-Id"].FirstOrDefault();
        return Guid.TryParse(userIdHeader, out var userId) ? userId : null;
    }
}

