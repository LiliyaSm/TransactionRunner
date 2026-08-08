using System.CommandLine;
using TransactionRunner.UseCases.DailyTransactions;

namespace TransactionRunner.Controllers
{
    internal class ProcessController
    {
        private const string DefaultRootFolder = "_task";
        private readonly IDailyTransactionsUseCase processor;
        public ProcessController(IDailyTransactionsUseCase processor)
        {
            this.processor = processor;
            Command.Arguments.Add(InputBalanceArg);
            Command.Arguments.Add(InputTransactionArg);
            Command.Arguments.Add(OutputBalanceArg);
            Command.Arguments.Add(DeclinedTransactionsArg);
        }

        private Argument<FileInfo> DeclinedTransactionsArg = new("declined_transactions")
        {
            Description = "The declined transactions file",
            DefaultValueFactory = _ => new FileInfo($"{DefaultRootFolder}/declined_transactions.csv")
        };

        private Argument<FileInfo> OutputBalanceArg = new("output_balance")
        {
            Description = "The output balance file",
            DefaultValueFactory = _ => new FileInfo($"{DefaultRootFolder}/output_balances.csv")
        };

        private Argument<FileInfo> InputTransactionArg = new("input_transactions")
        {
            Description = "The input transactions file",
            DefaultValueFactory = _ => new FileInfo($"{DefaultRootFolder}/transactions.csv")
        };

        private Argument<FileInfo> InputBalanceArg = new("input_balance")
        {
            Description = "The input balance file",
            DefaultValueFactory = _ => new FileInfo($"{DefaultRootFolder}/account_balances.csv")
        };

        public readonly Command Command = new("process", "Process daily transactions");

        public int Execute(ParseResult parseResult)
        {
            var inputBalanceFile = parseResult.GetValue(InputBalanceArg);
            if (inputBalanceFile == null || !inputBalanceFile.Exists)
            {
                Console.WriteLine($"Input balance file '{inputBalanceFile?.FullName}' does not exist.");
                return 1;
            }
            var inputTransactionsFile = parseResult.GetValue(InputTransactionArg);
            if (inputTransactionsFile == null || !inputTransactionsFile.Exists)
            {
                Console.WriteLine($"Input transactions file '{inputTransactionsFile?.FullName}' does not exist.");
                return 2;
            }
            var outputBalanceFile = parseResult.GetValue(OutputBalanceArg);
            if (outputBalanceFile == null)
            {
                Console.WriteLine("Output balance file is not specified.");
                return 3;
            }
            if (outputBalanceFile.Exists)
            {
                outputBalanceFile.Delete();
            }

            var declinedTransactionsFile = parseResult.GetValue(DeclinedTransactionsArg);
            if (declinedTransactionsFile == null)
            {
                Console.WriteLine("Declined transactions file is not specified.");
                return 4;
            }
            if (declinedTransactionsFile.Exists)
            {
                declinedTransactionsFile.Delete();
            }

            var result = processor.ProcessTransactions(inputBalanceFile, inputTransactionsFile, outputBalanceFile, declinedTransactionsFile);
            return result ? 0 : 5;
        }
    }
}
