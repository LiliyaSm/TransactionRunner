using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace TransactionRunner.Repositories.Balance
{

    internal class BalanceRepository : IBalanceRepository
    {
        public List<BalanceRecord> Read(FileInfo file)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var reader = new StreamReader(file.FullName))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<BalanceRecord>();
                return records.ToList();
            }
        }

        public void Write(FileInfo file, IEnumerable<BalanceRecord> balances)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var writer = new StreamWriter(file.FullName))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(balances);
            }
        }
    }
}
