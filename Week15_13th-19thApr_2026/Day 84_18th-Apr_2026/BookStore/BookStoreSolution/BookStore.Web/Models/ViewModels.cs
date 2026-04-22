namespace BookStore.Web.Models;

public class BookListItem
{
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public string ISBN { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public string CategoryName { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public string PublisherName { get; set; } = "";
}

public class OrderView
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "";
    public string CustomerName { get; set; } = "";
}

public class AuthResult
{
    public string Token { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "";
}

public class SalesReportView
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<TopBookView> TopBooks { get; set; } = new();
}

public class TopBookView
{
    public string Title { get; set; } = "";
    public int TotalSold { get; set; }
    public decimal Revenue { get; set; }
}

public class BookFormModel
{
    public string Title { get; set; } = "";
    public string ISBN { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int AuthorId { get; set; }
    public int PublisherId { get; set; }
}

public class CategoryView { public int CategoryId { get; set; } public string Name { get; set; } = ""; }
public class AuthorView { public int AuthorId { get; set; } public string Name { get; set; } = ""; }
public class PublisherView { public int PublisherId { get; set; } public string Name { get; set; } = ""; }
