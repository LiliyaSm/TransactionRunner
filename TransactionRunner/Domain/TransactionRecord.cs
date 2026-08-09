namespace TransactionRunner.Domain
{
    public class TransactionRecord
    {
        public long From { get; set; }
        public long To { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; private set; } = TransactionStatus.Pending;

        public bool HasValidAmount() => Amount > 0;

        public bool IsDeclined => Status == TransactionStatus.Declined;

        public void Accept() => Status = TransactionStatus.Accepted;

        public void Decline() => Status = TransactionStatus.Declined;
    }
}
