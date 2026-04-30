using System.Net.Sockets;
using System.Buffers.Binary;
using FramedMessage.Core;

if (args.Length < 1)
{
    Console.WriteLine("Required: FramedMessage.Client <input.bin> [host] [port]");
    return;
}

string inputFile = args[0];
string host = args.Length >= 2 ? args[1] : "localhost";
int port = 9000;
if (args.Length > 2 && !int.TryParse(args[2], out port))
{
    Console.Error.WriteLine($"Invalid port: '{args[2]}'");
    return;
}

if (!File.Exists(inputFile))
{
    Console.Error.WriteLine($"Input file '{inputFile}' does not exist.");
    return;
}

try
{
    using var file = File.OpenRead(inputFile);
    using var tcp = new TcpClient();

    Console.WriteLine($"Connecting to {host}:{port}...");
    await tcp.ConnectAsync(host, port);
    Console.WriteLine("connected");

    var stream = tcp.GetStream();
    var parser = new FrameParser(file);

    int count = 0;
    byte[]? payload;
    while ((payload = parser.ReadNext()) != null)
    {
        var header = new byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(header, (ushort)payload.Length);

        await stream.WriteAsync(header);
        await stream.WriteAsync(payload);

        count++;
        string preview = EncodingHelper.DecodeIso88592(payload);
        Console.WriteLine($"  [{count}] {preview[..Math.Min(50, preview.Length)]}");
    }

    tcp.Client.Shutdown(SocketShutdown.Send);
    Console.WriteLine($"done — {count} frames sent");
}
catch (FrameReadingException ex)
{
    Console.Error.WriteLine($"parse error: {ex.Message}");
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
}