using TransactionRunner.Domain;

namespace TransactionRunner.Repositories.Balance
{
    public interface IBalanceRepository
    {
        /// <summary>
        /// Reads balance records from the specified file and returns a list of BalanceRecord objects.
        /// </summary>
        /// <param name="file">The file to read balance records from.</param>
        /// <returns>A list of BalanceRecord objects.</returns>
        List<BalanceRecord> Read(string file);
        /// <summary>
        /// Writes the provided balance records to the specified file.
        /// </summary>
        /// <param name="file">The file to write balance records to.</param>
        /// <param name="balances">The balance records to write.</param>
        void Write(string file, IEnumerable<BalanceRecord> balances);
    }
}
