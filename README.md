# TransactionRunner

## Overview

A .NET CLI application that reads two CSV files — account balances and transactions — processes the transactions, and writes the result to output files. Transactions are declined if an account is unknown, the amount is non-positive, or the sender has insufficient funds. If any transaction is declined the output balance file is **not** generated; only the declined transactions file is written.

### Architecture

- **Domain** (`BalanceRecord`, `TransactionRecord`) — account/transaction models with validation, status, and transfer logic.
- **Controller** (`ProcessController`) — parses CLI arguments, validates that input files exist, and delegates to the use case.
- **Use Case** (`DailyTransactionsUseCase`) — orchestrates reading/writing via repositories.
- **Repositories** (`BalanceRepository`, `TransactionRepository`) — CSV I/O via CsvHelper `ClassMap`s (`BalanceRecordMap`, `TransactionRecordMap`).

### Dependencies

| Package | Purpose |
|---|---|
| `System.CommandLine` | CLI argument parsing |
| `CsvHelper` | CSV read/write |
| `Microsoft.Extensions.Hosting` | Dependency injection |

### Validation

- Input files must exist (checked by the controller before processing).
- Account IDs must be 16-digit numbers.
- Each account ID must appear exactly once in the balance file.
- Transaction amount must be greater than zero.
- The sender account must have a balance ≥ the transaction amount.
- Both the FROM and TO accounts must exist in the balance file.

### Tests

Unit tests (NUnit + Moq) cover:
- Happy path with chained transactions
- Overdraft / insufficient funds
- Negative transaction amount
- Unknown account reference
- Duplicate account ID in balance file

---

## Install .NET

Download and install the .NET SDK from [https://dot.net](https://dot.net).

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

---

## Build

```bash
dotnet build
```

---

## Run tests

From the repository root (the folder containing `TransactionRunner.slnx`):

```bash
dotnet test
```

---

## Calling the tool

### Default file locations

Place input files in the `_task/` folder inside the project directory:

```
TransactionRunner/_task/account_balances.csv
TransactionRunner/_task/transactions.csv
```

Then run from the `TransactionRunner/TransactionRunner/` directory (the one containing `TransactionRunner.csproj`):

```bash
dotnet run process
```

Output will be written to:

```
TransactionRunner/_task/output_balances.csv       # updated balances (only on full success)
TransactionRunner/_task/declined_transactions.csv  # declined transactions (only when failures occur)
```

### Custom file locations

Also from `TransactionRunner/TransactionRunner/`, pass file paths as arguments:

```bash
dotnet run process <balance_file> <transactions_file> <output_balance_file> <declined_file>
```

Example:

```bash
dotnet run process data/balances.csv data/txns.csv out/balances.csv out/declined.csv
```


---

## CSV File Format

Files have **no header row**. Columns are positional.

### account_balances.csv / output_balances.csv

| Column | Type | Description |
|---|---|---|
| 0 | `long` | Account ID |
| 1 | `decimal` | Account balance |

```
1000000000001001,5000.00
1000000000001002,250.50
1000000000001003,0.00
```

### transactions.csv / declined_transactions.csv

| Column | Type | Description |
|---|---|---|
| 0 | `long` | From account ID |
| 1 | `long` | To account ID |
| 2 | `decimal` | Amount |

```
1000000000001001,1000000000001002,200.00
1000000000001002,1000000000001003,50.50
```
---

## Processing Result

The tool processes transactions sequentially in the order they appear in the file. Each transaction immediately updates the in-memory balance, so a later transaction can use funds received by an earlier one in the same run.

- **All transactions accepted** → `output_balances.csv` is written; `declined_transactions.csv` is not created.
- **Any transaction declined** → `declined_transactions.csv` is written with all declined rows; `output_balances.csv` is **not** generated.

---

## Exit Codes

| Code | Name | Meaning |
|---|---|---|
| 0 | `Success` | All transactions processed; output balance written |
| 1 | `InputBalanceFileNotFound` | The input balance file does not exist |
| 2 | `InputTransactionsFileNotFound` | The input transactions file does not exist |
| 3 | `OutputBalanceFileNotSpecified` | Output balance file path could not be resolved |
| 4 | `DeclinedTransactionsFileNotSpecified` | Declined transactions file path could not be resolved |
| 5 | `ProcessingFailed` | One or more transactions were declined |

---

## Todo

- Better logging (structured, configurable log level)
- Better exception handling — cover repositories, controller and use case with proper error handling and custom exception types
- Add tests for `ProcessController` and `Repositories` test suites (empty files, malformed CSV)
