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
            var declinedTransactions = new List<TransactionRecord>();

            var balanceDictionary = GetGroupedBalances(incomingBalances);

            foreach (var transaction in transactions)
            {
                if (!balanceDictionary.ContainsKey(transaction.From))
                {
                    Console.WriteLine($"Skipping transaction from {transaction.From} to {transaction.To} for amount {transaction.Amount} due to unknown FROM account.");
                    declinedTransactions.Add(transaction);
                    continue;
                }
                if (!balanceDictionary.ContainsKey(transaction.To))
                {
                    Console.WriteLine($"Skipping transaction from {transaction.From} to {transaction.To} for amount {transaction.Amount} due to unknown TO account.");
                    declinedTransactions.Add(transaction);
                    continue;
                }
                var from = balanceDictionary[transaction.From];
                var to = balanceDictionary[transaction.To];
                if (IsValidTransaction(from, to, transaction.Amount))
                {
                    Console.WriteLine($"Processing transaction from {transaction.From} to {transaction.To} for amount {transaction.Amount}");
                    ApplyTransaction(from, to, transaction.Amount);
                }
                else
                {
                    Console.WriteLine($"Skipping transaction from {transaction.From} to {transaction.To} for amount {transaction.Amount} due to invalid amount or insufficient balance.");
                    declinedTransactions.Add(transaction);
                }
            }

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
            var groupedBalances = incomingBalances.GroupBy(x => x.AccountId);
            if (groupedBalances.Any(x => x.Count() != 1))
            {
                throw new InvalidOperationException("Each account should have exactly one balance record.");
            }
            var balanceDictionary = groupedBalances.ToDictionary(x => x.Key, x => x.Single());
            return balanceDictionary;
        }

        private bool IsValidTransaction(BalanceRecord from, BalanceRecord to, decimal amount)
        {
            return from.AccountBalance >= amount && amount > 0;
        }

        private void ApplyTransaction(BalanceRecord from, BalanceRecord to, decimal amount)
        {
            from.AccountBalance -= amount;
            to.AccountBalance += amount;
        }
    }
}
