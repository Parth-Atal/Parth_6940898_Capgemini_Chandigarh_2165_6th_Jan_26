namespace BookStore.Domain.Entities;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int BookId { get; set; }
    public int Qty { get; set; }
    public decimal Price { get; set; }
    public Order Order { get; set; } = null!;
    public Book Book { get; set; } = null!;
}