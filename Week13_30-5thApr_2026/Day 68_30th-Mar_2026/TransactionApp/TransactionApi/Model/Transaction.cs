namespace TransactionApi.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // Credit/Debit

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
