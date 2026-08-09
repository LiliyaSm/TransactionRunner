using CsvHelper.Configuration;
using TransactionRunner.Domain;

namespace TransactionRunner.Repositories.Transaction
{
    internal sealed class TransactionRecordMap : ClassMap<TransactionRecord>
    {
        public TransactionRecordMap()
        {
            Map(m => m.From).Index(0);
            Map(m => m.To).Index(1);
            Map(m => m.Amount).Index(2);
            Map(m => m.Status).Ignore();
        }
    }
}
