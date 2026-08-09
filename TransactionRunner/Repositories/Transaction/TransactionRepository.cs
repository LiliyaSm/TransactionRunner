using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using TransactionRunner.Domain;

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
                csv.Context.RegisterClassMap<TransactionRecordMap>();
                return csv.GetRecords<TransactionRecord>().ToList();
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
                csv.Context.RegisterClassMap<TransactionRecordMap>();
                csv.WriteRecords(transactions);
            }
        }
    }
}
