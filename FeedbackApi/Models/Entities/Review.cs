namespace FeedbackApi.Models.Entities;

public class Review
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ApartmentId { get; set; }
    public Guid BookingId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Apartment Apartment { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
}

