namespace BookStore.Web.Models;

public class CartItem
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Qty { get; set; }
}