using Moq;
using TransactionRunner.Repositories.Balance;
using TransactionRunner.Repositories.Transaction;
using TransactionRunner.UseCases.DailyTransactions;

namespace TransactionRunner.Tests.UseCases
{
    public class DailyTransactionsUseCaseTests
    {
        Mock<IBalanceRepository> balanceRepositoryMock;
        Mock<ITransactionRepository> transactionRepositoryMock;
        const string inputBalanceFile = "inputBalance.csv";
        const string outputBalanceFile = "outputBalance.csv";
        const string inputTransactionFile = "inputTransactions.csv";
        const string declinedTransactionFile = "declinedTransactions.csv";


        [SetUp]
        public void Setup()
        {
            balanceRepositoryMock = new Mock<IBalanceRepository>();
            transactionRepositoryMock = new Mock<ITransactionRepository>();
        }

        [Test]
        public void ProcessTransactions_GivenValidInput_ReturnsSuccess()
        {
            // Arrange
            List<BalanceRecord> capturedBalanceRecords = new();

            // mock the initial balance
            balanceRepositoryMock.Setup(repo => repo.Read(It.IsAny<string>()))
                .Returns(new List<BalanceRecord>
                {
                    new BalanceRecord { AccountId = 123, AccountBalance = 1000.00m },
                    new BalanceRecord { AccountId = 456, AccountBalance = 500.00m },
                    new BalanceRecord { AccountId = 789, AccountBalance = 300.00m }
                });
            // capture the balance records written to the output file
            balanceRepositoryMock.Setup(repo => repo.Write(It.IsAny<string>(), It.IsAny<List<BalanceRecord>>()))
                .Callback<string, IEnumerable<BalanceRecord>>((path, records) => capturedBalanceRecords = records.ToList());
            // mock the incoming transactions
            transactionRepositoryMock.Setup(repo => repo.Read(It.IsAny<string>()))
                .Returns(new List<TransactionRecord>
                {
                    new TransactionRecord { From = 123, To = 456, Amount = 100.00m },
                    // Check chaining of transactions / transaction simulation
                    new TransactionRecord { From = 456, To = 789, Amount = 600.00m }
                });

            var useCase = new DailyTransactionsUseCase(balanceRepositoryMock.Object, transactionRepositoryMock.Object);

            // Act
            var result = useCase.ProcessTransactions(inputBalanceFile, inputTransactionFile, outputBalanceFile, declinedTransactionFile);

            // Assert
            balanceRepositoryMock.Verify(repo => repo.Write(outputBalanceFile, It.IsAny<List<BalanceRecord>>()), Times.Once);
            transactionRepositoryMock.Verify(repo => repo.Write(It.IsAny<string>(), It.IsAny<List<TransactionRecord>>()), Times.Never);
            Assert.That(capturedBalanceRecords.First(x => x.AccountId == 123).AccountBalance, Is.EqualTo(900.00m));
            Assert.That(capturedBalanceRecords.First(x => x.AccountId == 456).AccountBalance, Is.EqualTo(0.00m));
            Assert.That(capturedBalanceRecords.First(x => x.AccountId == 789).AccountBalance, Is.EqualTo(900.00m));
            Assert.That(capturedBalanceRecords.Count, Is.EqualTo(3));
        }

        [Test]
        public void ProcessTransactions_GivenValidNegativeBalanceOperation_Fails()
        {
            // Arrange
            List<TransactionRecord> capturedTransactionRecords = new();

            // mock the initial balance
            balanceRepositoryMock.Setup(repo => repo.Read(It.IsAny<string>()))
                .Returns(new List<BalanceRecord>
                {
                    new BalanceRecord { AccountId = 123, AccountBalance = 1000.00m },
                    new BalanceRecord { AccountId = 456, AccountBalance = 500.00m }
                });
            // capture the balance records written to the output file
            transactionRepositoryMock.Setup(repo => repo.Write(It.IsAny<string>(), It.IsAny<List<TransactionRecord>>()))
                .Callback<string, IEnumerable<TransactionRecord>>((path, records) => capturedTransactionRecords = records.ToList());
            // mock the incoming transactions
            transactionRepositoryMock.Setup(repo => repo.Read(It.IsAny<string>()))
                .Returns(new List<TransactionRecord>
                {
                    new TransactionRecord { From = 123, To = 456, Amount = 1100.00m }
                });

            var useCase = new DailyTransactionsUseCase(balanceRepositoryMock.Object, transactionRepositoryMock.Object);

            // Act
            var result = useCase.ProcessTransactions(inputBalanceFile, inputTransactionFile, outputBalanceFile, declinedTransactionFile);

            // Assert
            balanceRepositoryMock.Verify(repo => repo.Write(It.IsAny<string>(), It.IsAny<List<BalanceRecord>>()), Times.Never);
            transactionRepositoryMock.Verify(repo => repo.Write(declinedTransactionFile, It.IsAny<List<TransactionRecord>>()), Times.Once);
            Assert.That(capturedTransactionRecords.First(x => x.From == 123).Amount, Is.EqualTo(1100.00m));
            Assert.That(capturedTransactionRecords.Count, Is.EqualTo(1));
        }

        [Test]
        public void ProcessTransactions_GivenDuplicateAccounts_Fails()
        {
            // Arrange
            List<TransactionRecord> capturedTransactionRecords = new();

            // mock the initial balance
            balanceRepositoryMock.Setup(repo => repo.Read(It.IsAny<string>()))
                .Returns(new List<BalanceRecord>
                {
                    new BalanceRecord { AccountId = 123, AccountBalance = 1000.00m },
                    new BalanceRecord { AccountId = 123, AccountBalance = 500.00m }
                });

            var useCase = new DailyTransactionsUseCase(balanceRepositoryMock.Object, transactionRepositoryMock.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => useCase.ProcessTransactions(inputBalanceFile, inputTransactionFile, outputBalanceFile, declinedTransactionFile));
        }

        [Test]
        public void ProcessTransactions_GivenNegativeTransactionAmount_Fails()
        {
            // Arrange
            List<TransactionRecord> capturedTransactionRecords = new();

            // mock the initial balance
            balanceRepositoryMock.Setup(repo => repo.Read(It.IsAny<string>()))
                .Returns(new List<BalanceRecord>
                {
                    new BalanceRecord { AccountId = 123, AccountBalance = 1000.00m },
                    new BalanceRecord { AccountId = 456, AccountBalance = 500.00m }
                });
            // capture the balance records written to the output file
            transactionRepositoryMock.Setup(repo => repo.Write(It.IsAny<string>(), It.IsAny<List<TransactionRecord>>()))
                .Callback<string, IEnumerable<TransactionRecord>>((path, records) => capturedTransactionRecords = records.ToList());
            // mock the incoming transactions
            transactionRepositoryMock.Setup(repo => repo.Read(It.IsAny<string>()))
                .Returns(new List<TransactionRecord>
                {
                    new TransactionRecord { From = 123, To = 456, Amount = -100.00m }
                });

            var useCase = new DailyTransactionsUseCase(balanceRepositoryMock.Object, transactionRepositoryMock.Object);

            // Act
            var result = useCase.ProcessTransactions(inputBalanceFile, inputTransactionFile, outputBalanceFile, declinedTransactionFile);

            // Assert
            balanceRepositoryMock.Verify(repo => repo.Write(It.IsAny<string>(), It.IsAny<List<BalanceRecord>>()), Times.Never);
            transactionRepositoryMock.Verify(repo => repo.Write(declinedTransactionFile, It.IsAny<List<TransactionRecord>>()), Times.Once);
            Assert.That(capturedTransactionRecords.First(x => x.From == 123).Amount, Is.EqualTo(-100.00m));
            Assert.That(capturedTransactionRecords.Count, Is.EqualTo(1));
        }

        [Test]
        public void ProcessTransactions_GivenUnknownAccount_Fails()
        {
            // Arrange
            List<TransactionRecord> capturedTransactionRecords = new();

            // mock the initial balance
            balanceRepositoryMock.Setup(repo => repo.Read(It.IsAny<string>()))
                .Returns(new List<BalanceRecord>
                {
                    new BalanceRecord { AccountId = 123, AccountBalance = 1000.00m },
                    new BalanceRecord { AccountId = 456, AccountBalance = 500.00m }
                });
            // capture the balance records written to the output file
            transactionRepositoryMock.Setup(repo => repo.Write(It.IsAny<string>(), It.IsAny<List<TransactionRecord>>()))
                .Callback<string, IEnumerable<TransactionRecord>>((path, records) => capturedTransactionRecords = records.ToList());
            // mock the incoming transactions
            transactionRepositoryMock.Setup(repo => repo.Read(It.IsAny<string>()))
                .Returns(new List<TransactionRecord>
                {
                    new TransactionRecord { From = 123, To = 456, Amount = 100.00m },
                    // NO-OP transactions with unknown account
                    new TransactionRecord { From = 789, To = 456, Amount = 10000.00m },
                    new TransactionRecord { From = 123, To = 789, Amount = 100.00m }
                });

            var useCase = new DailyTransactionsUseCase(balanceRepositoryMock.Object, transactionRepositoryMock.Object);

            // Act
            var result = useCase.ProcessTransactions(inputBalanceFile, inputTransactionFile, outputBalanceFile, declinedTransactionFile);

            // Assert
            balanceRepositoryMock.Verify(repo => repo.Write(It.IsAny<string>(), It.IsAny<List<BalanceRecord>>()), Times.Never);
            transactionRepositoryMock.Verify(repo => repo.Write(declinedTransactionFile, It.IsAny<List<TransactionRecord>>()), Times.Once);
            Assert.That(capturedTransactionRecords.First(x => x.From == 789).Amount, Is.EqualTo(10000.00m));
            Assert.That(capturedTransactionRecords.First(x => x.From == 123).Amount, Is.EqualTo(100.00m));
            Assert.That(capturedTransactionRecords.Count, Is.EqualTo(2));
        }
    }
}
