# TransactionRunner

## Overview

### Design

A CLI application that reads two files - balance and transactions and output analysis result. Domain models are used to encapsulate business logic when possible. Application will output the resulting balance if processed successfully or error messages if any validation fails.

### Dependencies

- CLI parser
- CSV parser

### Validation

File types / size
Model validation for input raws
Business rules for transactions

### Tests

Happy path, edge cases, validation

## Calling the tool

You can use default file location expecting the following files to be present:
"_tast/account_balances.csv"
"_task/transactions.csv"
And	the output will be generated in the following files:
"_task/output_balances.csv"
"_task/declined_transactions.csv"

By calling "TransactionRunner.exe process"

Or you can specify the file locations by calling "TransactionRunner.exe process <balance_file_path> <transaction_file_path> <output_file_path> <declined_file_path>"

## Processing Result
Transaction runner will read the balance file and transaction file, process the transactions and output the resulting balance to the output file. If any transactions are declined, they will be written to the declined file and no resulting balance will be generated.

## Todo

- Better logging
- Better exception handling (cover Repos, Controller, UseCase with proper error handling, add custom errors)
- Add tests for Controller and UseCase

