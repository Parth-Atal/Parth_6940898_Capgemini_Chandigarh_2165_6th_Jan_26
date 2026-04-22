namespace BookStore.Application.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string toEmail, int orderId, decimal total);
    Task SendLowStockAlertAsync(string bookTitle, int currentStock);
    Task SendOrderReceivedToAdminAsync(string adminEmail, int orderId, string customerName, decimal total);
    Task SendOrderStatusUpdateToCustomerAsync(string customerEmail, int orderId, string customerName, string newStatus);
}