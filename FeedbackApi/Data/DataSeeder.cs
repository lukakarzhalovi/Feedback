using FeedbackApi.Models.Entities;
using FeedbackApi.Models.Enums;

namespace FeedbackApi.Data;

public static class DataSeeder
{
    public static void Seed(FeedbackDbContext context)
    {
        if (context.Users.Any())
            return;

        // Users
        var users = new List<User>
        {
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "John Doe" },
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Jane Smith" },
            new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Bob Wilson" }
        };
        context.Users.AddRange(users);

        // Apartments
        var apartments = new List<Apartment>
        {
            new() { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Address = "123 Ocean View, Miami, FL 33139" },
            new() { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Address = "456 Mountain Lodge, Denver, CO 80202" },
            new() { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Address = "789 City Center, New York, NY 10001" }
        };
        context.Apartments.AddRange(apartments);

        // Bookings - mix of completed and active
        var bookings = new List<Booking>
        {
            // Completed bookings (can review)
            new()
            {
                Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                UserId = users[0].Id,
                ApartmentId = apartments[0].Id,
                CheckInDate = DateTime.UtcNow.AddDays(-10),
                CheckOutDate = DateTime.UtcNow.AddDays(-5),
                Status = BookingStatus.CheckedOut
            },
            new()
            {
                Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                UserId = users[1].Id,
                ApartmentId = apartments[1].Id,
                CheckInDate = DateTime.UtcNow.AddDays(-15),
                CheckOutDate = DateTime.UtcNow.AddDays(-10),
                Status = BookingStatus.CheckedOut
            },
            new()
            {
                Id = Guid.Parse("d3333333-3333-3333-3333-333333333333"),
                UserId = users[2].Id,
                ApartmentId = apartments[2].Id,
                CheckInDate = DateTime.UtcNow.AddDays(-20),
                CheckOutDate = DateTime.UtcNow.AddDays(-15),
                Status = BookingStatus.CheckedOut
            },
            // Active booking (cannot review yet)
            new()
            {
                Id = Guid.Parse("d4444444-4444-4444-4444-444444444444"),
                UserId = users[0].Id,
                ApartmentId = apartments[1].Id,
                CheckInDate = DateTime.UtcNow.AddDays(-2),
                CheckOutDate = DateTime.UtcNow.AddDays(3),
                Status = BookingStatus.CheckedIn
            },
            // Future booking (cannot review)
            new()
            {
                Id = Guid.Parse("d5555555-5555-5555-5555-555555555555"),
                UserId = users[1].Id,
                ApartmentId = apartments[2].Id,
                CheckInDate = DateTime.UtcNow.AddDays(5),
                CheckOutDate = DateTime.UtcNow.AddDays(10),
                Status = BookingStatus.Confirmed
            }
        };
        context.Bookings.AddRange(bookings);

        context.SaveChanges();
    }
}

