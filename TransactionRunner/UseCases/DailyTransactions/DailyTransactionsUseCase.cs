using TransactionRunner.Domain;
using TransactionRunner.Repositories.Balance;
using TransactionRunner.Repositories.Transaction;

namespace TransactionRunner.UseCases.DailyTransactions
{

    public class DailyTransactionsUseCase : IDailyTransactionsUseCase
    {
        private readonly IBalanceRepository balanceRepository;
        private readonly ITransactionRepository transactionRepository;

        public DailyTransactionsUseCase(IBalanceRepository balanceRepository, ITransactionRepository transactionRepository)
        {
            this.balanceRepository = balanceRepository;
            this.transactionRepository = transactionRepository;
        }

        public bool ProcessTransactions(string inputBalanceFile, string inputTransactionsFile, string outputBalanceFile, string declinedTransactionFile)
        {
            Console.WriteLine($"Processing transactions from {inputTransactionsFile} based on balances in {inputBalanceFile} and saving results to {outputBalanceFile}");

            var incomingBalances = balanceRepository.Read(inputBalanceFile);
            var transactions = transactionRepository.Read(inputTransactionsFile);

            var balanceDictionary = GetGroupedBalances(incomingBalances);

            foreach (var transaction in transactions)
            {
                if (!balanceDictionary.ContainsKey(transaction.From))
                {
                    Console.WriteLine($"Skipping transaction from {transaction.From} to {transaction.To} for amount {transaction.Amount} due to unknown FROM account.");
                    transaction.Decline();
                    continue;
                }
                if (!balanceDictionary.ContainsKey(transaction.To))
                {
                    Console.WriteLine($"Skipping transaction from {transaction.From} to {transaction.To} for amount {transaction.Amount} due to unknown TO account.");
                    transaction.Decline();
                    continue;
                }
                var from = balanceDictionary[transaction.From];
                var to = balanceDictionary[transaction.To];
                if (transaction.HasValidAmount() && from.CanDebit(transaction.Amount))
                {
                    Console.WriteLine($"Processing transaction from {transaction.From} to {transaction.To} for amount {transaction.Amount}");
                    from.Debit(transaction.Amount);
                    to.Credit(transaction.Amount);
                    transaction.Accept();
                }
                else
                {
                    Console.WriteLine($"Skipping transaction from {transaction.From} to {transaction.To} for amount {transaction.Amount} due to invalid amount or insufficient balance.");
                    transaction.Decline();
                }
            }

            var declinedTransactions = transactions.Where(t => t.IsDeclined).ToList();
            if (declinedTransactions.Any())
            {
                Console.WriteLine("Some transactions were declined. Output balance won't be generated. Writing declined transactions to file.");
                transactionRepository.Write(declinedTransactionFile, declinedTransactions);
                return false;
            }
            else
            {
                Console.WriteLine("All transactions processed successfully. Writing updated balances to file.");
                balanceRepository.Write(outputBalanceFile, balanceDictionary.Values.ToList());
                return true;
            }
        }

        private static Dictionary<long, BalanceRecord> GetGroupedBalances(List<BalanceRecord> incomingBalances)
        {
            if (incomingBalances.Any(x => !x.HasValidAccountId()))
            {
                throw new InvalidOperationException("Account IDs must be 16-digit numbers.");
            }
            var groupedBalances = incomingBalances.GroupBy(x => x.AccountId);
            if (groupedBalances.Any(x => x.Count() != 1))
            {
                throw new InvalidOperationException("Each account should have exactly one balance record.");
            }
            return groupedBalances.ToDictionary(x => x.Key, x => x.Single());
        }
    }
}
