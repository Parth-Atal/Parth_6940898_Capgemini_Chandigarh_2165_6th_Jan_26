namespace BookStore.Domain.Entities;

public class EmailLog
{
    public int EmailLogId { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime SentDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Sent";
}