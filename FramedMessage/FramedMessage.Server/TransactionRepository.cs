using FramedMessage.Core;
using Microsoft.Data.Sqlite;

namespace FramedMessage.Server;

public class TransactionRepository : IDisposable, IAsyncDisposable
{
    private readonly SqliteConnection _db;

    public TransactionRepository(string dbPath = "transactions.db")
    {
        _db = new SqliteConnection($"Data Source={dbPath}");
        _db.Open();
        
        using var cmd = _db.CreateCommand();
        cmd.CommandText = File.ReadAllText("schema.sql");
        cmd.ExecuteNonQuery();
    }

    public void Save(TransactionRecord record)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = """
                          INSERT INTO transactions 
                              (type, transaction_id, amount, currency, raw_message, received_at)
                          VALUES 
                              (@type, @txnId, @amount, @currency, @raw, @received)
                          """;

        cmd.Parameters.AddWithValue("@type", record.Type);
        cmd.Parameters.AddWithValue("@txnId", (object?)record.TransactionId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@amount", (object?)record.Amount ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@currency", (object?)record.Currency ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@raw", record.RawMessage);
        cmd.Parameters.AddWithValue("@received", record.ReceivedAt.ToString("o"));

        cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _db.DisposeAsync();
    }
}