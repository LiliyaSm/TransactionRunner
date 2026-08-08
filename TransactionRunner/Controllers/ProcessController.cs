using System.CommandLine;
using TransactionRunner.UseCases.DailyTransactions;

namespace TransactionRunner.Controllers
{
    public enum ProcessResult
    {
        Success = 0,
        InputBalanceFileNotFound = 1,
        InputTransactionsFileNotFound = 2,
        OutputBalanceFileNotSpecified = 3,
        DeclinedTransactionsFileNotSpecified = 4,
        ProcessingFailed = 5
    }

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

        /// <summary>
        /// Executes the process of handling daily transactions based on the provided command-line arguments.
        /// </summary>
        /// <param name="parseResult">CLI arguments parse result</param>
        /// <returns>Success if processed, otherwise an error code</returns>
        public ProcessResult Execute(ParseResult parseResult)
        {
            var inputBalanceFile = parseResult.GetValue(InputBalanceArg);
            if (inputBalanceFile == null || !inputBalanceFile.Exists)
            {
                Console.WriteLine($"Input balance file '{inputBalanceFile?.FullName}' does not exist.");
                return ProcessResult.InputBalanceFileNotFound;
            }
            var inputTransactionsFile = parseResult.GetValue(InputTransactionArg);
            if (inputTransactionsFile == null || !inputTransactionsFile.Exists)
            {
                Console.WriteLine($"Input transactions file '{inputTransactionsFile?.FullName}' does not exist.");
                return ProcessResult.InputTransactionsFileNotFound;
            }
            var outputBalanceFile = parseResult.GetValue(OutputBalanceArg);
            if (outputBalanceFile == null)
            {
                Console.WriteLine("Output balance file is not specified.");
                return ProcessResult.OutputBalanceFileNotSpecified;
            }
            if (outputBalanceFile.Exists)
            {
                outputBalanceFile.Delete();
            }

            var declinedTransactionsFile = parseResult.GetValue(DeclinedTransactionsArg);
            if (declinedTransactionsFile == null)
            {
                Console.WriteLine("Declined transactions file is not specified.");
                return ProcessResult.DeclinedTransactionsFileNotSpecified;
            }   
            if (declinedTransactionsFile.Exists)
            {
                declinedTransactionsFile.Delete();
            }

            var result = processor.ProcessTransactions(inputBalanceFile.FullName, inputTransactionsFile.FullName, outputBalanceFile.FullName, declinedTransactionsFile.FullName);
            return result ? ProcessResult.Success : ProcessResult.ProcessingFailed;
        }
    }
}
