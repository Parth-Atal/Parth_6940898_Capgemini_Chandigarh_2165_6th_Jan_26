namespace BookStore.Application.DTOs;

public class OrderCreateDto
{
    public List<OrderItemCreateDto> Items { get; set; } = new();
}

public class OrderItemCreateDto
{
    public int BookId { get; set; }
    public int Qty { get; set; }
}

public class OrderResponseDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string BookTitle { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal Price { get; set; }
}