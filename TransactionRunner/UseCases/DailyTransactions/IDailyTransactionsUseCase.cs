using Microsoft.Extensions.FileProviders;

namespace TransactionRunner.UseCases.DailyTransactions
{
    public interface IDailyTransactionsUseCase
    {
        bool ProcessTransactions(string inputBalanceFile, string inputTransactionsFile, string outputBalanceFile, string declinedTransactionFile);
    }
}
