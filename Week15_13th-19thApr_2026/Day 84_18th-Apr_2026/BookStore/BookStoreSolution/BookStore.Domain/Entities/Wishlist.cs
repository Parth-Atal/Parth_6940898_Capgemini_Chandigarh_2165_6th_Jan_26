namespace BookStore.Domain.Entities;

public class Wishlist
{
    public int UserId { get; set; }
    public int BookId { get; set; }
    public User User { get; set; } = null!;
    public Book Book { get; set; } = null!;
}