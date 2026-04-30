using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using FramedMessage.Core;

namespace FramedMessage.Tests;

public class IntegrationTests
{
    [Fact]
    public async Task ClientToServer_FramesDecodedCorrectly()
    {
        var messages = new[]
        {
            "PAYMENT|123|100.00|HRK",
            "UPLATA|ČĆŽŠĐ|100.50|HRK",
            "END"
        };

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;

        var serverTask = Task.Run(async () =>
        {
            using var client = await listener.AcceptTcpClientAsync();
            var parser = new FrameParser(client.GetStream());
            var received = new List<string>();
            byte[]? payload;
            while ((payload = parser.ReadNext()) != null)
                received.Add(EncodingHelper.DecodeIso88592(payload));
            return received;
        });

        using var tcp = new TcpClient();
        await tcp.ConnectAsync(IPAddress.Loopback, port);
        var stream = tcp.GetStream();

        foreach (var msg in messages)
        {
            var payload = EncodingHelper.EncodeIso88592(msg);
            var header = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(header, (ushort)payload.Length);
            await stream.WriteAsync(header);
            await stream.WriteAsync(payload);
        }

        tcp.Client.Shutdown(SocketShutdown.Send);

        var result = await serverTask;
        listener.Stop();

        Assert.Equal(messages, result);
    }
}
