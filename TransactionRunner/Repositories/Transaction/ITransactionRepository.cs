using Microsoft.Extensions.FileProviders;

namespace TransactionRunner.Repositories.Transaction
{
    public interface ITransactionRepository
    {
        List<TransactionRecord> Read(string file);
        void Write(string file, IEnumerable<TransactionRecord> transactions);
    }
}
