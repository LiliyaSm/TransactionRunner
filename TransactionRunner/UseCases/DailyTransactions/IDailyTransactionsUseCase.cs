namespace TransactionRunner.UseCases.DailyTransactions
{
    public interface IDailyTransactionsUseCase
    {
        /// <summary>
        /// Processes daily transactions based on the provided file paths.
        /// </summary>
        /// <param name="inputBalanceFile"> source of account balance information </param>
        /// <param name="inputTransactionsFile"> source of transaction data </param>
        /// <param name="outputBalanceFile"> destination for updated balance information </param>
        /// <param name="declinedTransactionFile"> destination for declined transaction records </param>
        /// <returns> True if all transactions are accepted, False if any are declined </returns>
        bool ProcessTransactions(string inputBalanceFile, string inputTransactionsFile, string outputBalanceFile, string declinedTransactionFile);
    }
}
