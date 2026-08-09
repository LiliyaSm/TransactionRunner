namespace TransactionRunner.Domain
{
    public class BalanceRecord
    {
        private const long MinSixteenDigitAccountId = 1_000_000_000_000_000;
        private const long MaxSixteenDigitAccountId = 9_999_999_999_999_999;

        public long AccountId { get; set; }
        public decimal AccountBalance { get; set; }

        public bool HasValidAccountId() =>
            AccountId is >= MinSixteenDigitAccountId and <= MaxSixteenDigitAccountId;

        public bool CanDebit(decimal amount) => AccountBalance >= amount;

        public void Debit(decimal amount) => AccountBalance -= amount;

        public void Credit(decimal amount) => AccountBalance += amount;
    }
}
