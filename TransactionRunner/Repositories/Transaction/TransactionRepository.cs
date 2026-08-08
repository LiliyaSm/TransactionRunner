using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Globalization;

namespace TransactionRunner.Repositories.Transaction
{

    public class TransactionRepository : ITransactionRepository
    {
        public List<TransactionRecord> Read(string file)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<TransactionRecord>();
                return records.ToList();
            }
        }

        public void Write(string file, IEnumerable<TransactionRecord> transactions)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(transactions);
            }
        }
    }
}
