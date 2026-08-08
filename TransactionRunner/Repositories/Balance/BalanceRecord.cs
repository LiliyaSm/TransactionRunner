using CsvHelper.Configuration.Attributes;

namespace TransactionRunner.Repositories.Balance
{
    public class BalanceRecord
    {
        [Index(0)]
        public long AccountId { get; set; }
        [Index(1)]
        public decimal AccountBalance { get; set; }
    }
}
