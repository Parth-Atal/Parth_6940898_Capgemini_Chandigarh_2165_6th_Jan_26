namespace BookStore.Domain.Entities;

public class UserProfile
{
    public int ProfileId { get; set; }
    public int UserId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}