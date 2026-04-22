namespace BookStore.Domain.Entities;

public class Publisher
{
    public int PublisherId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Book> Books { get; set; } = new List<Book>();
}