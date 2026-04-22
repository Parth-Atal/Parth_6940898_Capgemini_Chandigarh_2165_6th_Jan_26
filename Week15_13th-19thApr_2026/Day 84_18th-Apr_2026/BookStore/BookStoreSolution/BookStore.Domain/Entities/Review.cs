namespace BookStore.Domain.Entities;

public class Review
{
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public Book Book { get; set; } = null!;
}