namespace BookStore.Application.DTOs;

public class SalesReportDto
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<TopSellingBookDto> TopBooks { get; set; } = new();
}

public class TopSellingBookDto
{
    public string Title { get; set; } = string.Empty;
    public int TotalSold { get; set; }
    public decimal Revenue { get; set; }
}