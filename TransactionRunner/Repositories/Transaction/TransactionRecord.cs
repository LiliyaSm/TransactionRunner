using CsvHelper.Configuration.Attributes;

namespace TransactionRunner.Repositories.Transaction
{
    public class TransactionRecord
    {
        [Index(0)]
        public long From { get; set; }
        [Index(1)]
        public long To { get; set; }
        [Index(2)]
        public decimal Amount { get; set; }
    }
}
