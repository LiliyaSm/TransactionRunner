using CsvHelper.Configuration;
using TransactionRunner.Domain;

namespace TransactionRunner.Repositories.Balance
{
    internal sealed class BalanceRecordMap : ClassMap<BalanceRecord>
    {
        public BalanceRecordMap()
        {
            Map(m => m.AccountId).Index(0);
            Map(m => m.AccountBalance).Index(1);
        }
    }
}
