using FeedbackApi.Models.Enums;

namespace FeedbackApi.Models.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ApartmentId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public BookingStatus Status { get; set; }

    public User User { get; set; } = null!;
    public Apartment Apartment { get; set; } = null!;
    public Review? Review { get; set; }
}

