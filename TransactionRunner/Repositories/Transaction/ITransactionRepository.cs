namespace TransactionRunner.Repositories.Transaction
{
    public interface ITransactionRepository
    {
        List<TransactionRecord> Read(FileInfo file);
        void Write(FileInfo file, IEnumerable<TransactionRecord> transactions);
    }
}
