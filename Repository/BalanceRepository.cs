using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace TransactionRunner.Repository
{
    public class BalanceRecord
    {
        [Index(0)]
        public long AccountId { get; set; }
        [Index(1)]
        public decimal AccountBalance { get; set; }
    }
    internal class BalanceRepository
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
