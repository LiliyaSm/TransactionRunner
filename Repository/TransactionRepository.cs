using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace TransactionRunner.Repository
{
    public class TransactionRecord
    {
        [Index(0)]
        public long From { get; set; }
        [Index(1)]
        public long To { get; set; }
        [Index(2)]
        public decimal Amount { get; set; }
    }
    public class TransactionRepository
    {
        public List<TransactionRecord> Read(FileInfo file)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var reader = new StreamReader(file.FullName))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<TransactionRecord>();
                return records.ToList();
            }
        }

        public void Write(FileInfo file, IEnumerable<TransactionRecord> transactions)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var writer = new StreamWriter(file.FullName))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(transactions);
            }
        }
    }
}
