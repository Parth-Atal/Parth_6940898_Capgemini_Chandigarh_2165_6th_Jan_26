using BookStore.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookStore.Application.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger) => _logger = logger;

    public Task SendOrderConfirmationAsync(string toEmail, int orderId, decimal total)
    {
        _logger.LogInformation("Order confirmation email sent to {Email} for Order #{OrderId}, Total: {Total}", toEmail, orderId, total);
        return Task.CompletedTask;
    }

    public Task SendLowStockAlertAsync(string bookTitle, int currentStock)
    {
        _logger.LogWarning("Low stock alert: \"{Title}\" has only {Stock} left.", bookTitle, currentStock);
        return Task.CompletedTask;
    }

    public Task SendOrderReceivedToAdminAsync(string adminEmail, int orderId, string customerName, decimal total)
    {
        _logger.LogInformation(
            "Admin notification email sent to {AdminEmail}: New Order #{OrderId} from {CustomerName}, Total: {Total}",
            adminEmail, orderId, customerName, total);
        return Task.CompletedTask;
    }

    public Task SendOrderStatusUpdateToCustomerAsync(string customerEmail, int orderId, string customerName, string newStatus)
    {
        _logger.LogInformation(
            "Order status update email sent to {CustomerEmail}: Order #{OrderId} status changed to {Status}",
            customerEmail, orderId, newStatus);
        return Task.CompletedTask;
    }
}