namespace BookStore.Application.DTOs;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class ReviewCreateDto
{
    public int BookId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}