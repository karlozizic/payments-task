using System.Net;
using System.Net.Sockets;
using FramedMessage.Core;
using FramedMessage.Server;

int port = 9000;
if (args.Length > 0 && !int.TryParse(args[0], out port))
{
    Console.Error.WriteLine($"Invalid port: '{args[0]}'");
    return;
}
string outputFile = args.Length > 1 ? args[1] : "output.txt";

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine($"Server listening on port {port}");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    var endpoint = client.Client.RemoteEndPoint;
    Console.WriteLine($"Accepted connection from {endpoint}");
    
    _ = HandleClient(client, outputFile, endpoint);
}

async Task HandleClient(TcpClient client, string outputPath, EndPoint? endpoint)
{
    using var _ = client;
    var parser = new FrameParser(client.GetStream());

    int count = 0;

    try
    {
        using var repository = new TransactionRepository();
        using var writer = new StreamWriter(outputPath, append: true, encoding: System.Text.Encoding.UTF8);

        byte[]? payload;
        while ((payload = parser.ReadNext()) != null)
        {
            string message = EncodingHelper.DecodeIso88592(payload);
            count++;

            Console.WriteLine($"  [{count}] {message}");

            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
            
            var record = TransactionRecord.Parse(message);
            repository.Save(record);
        }

        Console.WriteLine($"{endpoint} closed ({count} frames)");
    }
    catch (FrameReadingException ex)
    {
        Console.Error.WriteLine($"Frame reading error from {endpoint}: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error handling client {endpoint}: {ex.Message}");
    }
}