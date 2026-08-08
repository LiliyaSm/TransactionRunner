namespace TransactionRunner.UseCases.DailyTransactions
{
    public interface IDailyTransactionsUseCase
    {
        bool ProcessTransactions(FileInfo inputBalanceFile, FileInfo inputTransactionsFile, FileInfo outputBalanceFile, FileInfo declinedTransactionFile);
    }
}
