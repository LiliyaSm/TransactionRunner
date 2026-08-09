using TransactionRunner.Domain;

namespace TransactionRunner.Repositories.Transaction
{
    public interface ITransactionRepository
    {
        /// <summary>
        /// Reads transaction records from the specified file and returns a list of TransactionRecord objects.
        /// </summary>
        /// <param name="file">The file to read transaction records from.</param>
        /// <returns>A list of TransactionRecord objects.</returns>
        List<TransactionRecord> Read(string file);
        /// <summary>
        /// Writes the provided transaction records to the specified file.
        /// </summary>
        /// <param name="file">The file to write transaction records to.</param>
        /// <param name="transactions">The transaction records to write.</param>
        void Write(string file, IEnumerable<TransactionRecord> transactions);
    }
}
