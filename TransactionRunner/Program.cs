using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using TransactionRunner.Controllers;
using TransactionRunner.Repositories.Transaction;
using TransactionRunner.Repositories.Balance;
using TransactionRunner.UseCases.DailyTransactions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<ProcessController>();
builder.Services.AddSingleton<IDailyTransactionsUseCase, DailyTransactionsUseCase>();
builder.Services.AddSingleton<IBalanceRepository, BalanceRepository>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
var host = builder.Build();

var processController = host.Services.GetRequiredService<ProcessController>();
processController.Command.SetAction(processController.Execute);

var rootCommand = new RootCommand("Transaction Runner CLI");
rootCommand.Subcommands.Add(processController.Command);

return rootCommand.Parse(args).Invoke();

