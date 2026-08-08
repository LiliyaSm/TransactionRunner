namespace TransactionRunner.Repositories.Balance
{
    public interface IBalanceRepository
    {
        List<BalanceRecord> Read(FileInfo file);
        void Write(FileInfo file, IEnumerable<BalanceRecord> balances);
    }
}
