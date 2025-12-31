namespace FeedbackApi.Models.Entities;

public class Apartment
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;

    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}

