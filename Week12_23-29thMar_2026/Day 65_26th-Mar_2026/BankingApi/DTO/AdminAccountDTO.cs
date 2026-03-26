namespace BankingApi.DTOs
{
    public class AdminAccountDTO
    {
        public string AccountHolderName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }
}