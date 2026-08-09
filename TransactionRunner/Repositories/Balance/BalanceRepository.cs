using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using TransactionRunner.Domain;

namespace TransactionRunner.Repositories.Balance
{

    internal class BalanceRepository : IBalanceRepository
    {
        public List<BalanceRecord> Read(string file)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<BalanceRecordMap>();
                return csv.GetRecords<BalanceRecord>().ToList();
            }
        }

        public void Write(string file, IEnumerable<BalanceRecord> balances)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<BalanceRecordMap>();
                csv.WriteRecords(balances);
            }
        }
    }
}
