using Microsoft.Extensions.FileProviders;

namespace TransactionRunner.Repositories.Balance
{
    public interface IBalanceRepository
    {
        List<BalanceRecord> Read(string file);
        void Write(string file, IEnumerable<BalanceRecord> balances);
    }
}
