using System.Globalization;

namespace FramedMessage.Core;

public class TransactionRecord
{
    public string Type { get; set; } = "";
    public string? TransactionId { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string RawMessage { get; set; } = "";
    public DateTime ReceivedAt { get; } = DateTime.UtcNow;

    public static TransactionRecord Parse(string message)
    {
        var parts = message.Split('|');

        var record = new TransactionRecord
        {
            Type = parts[0],
            RawMessage = message
        };

        if (parts.Length >= 4)
        {
            record.TransactionId = parts[1];
            if (decimal.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                record.Amount = amount;
            record.Currency = parts[3];
        }

        return record;
    }
}