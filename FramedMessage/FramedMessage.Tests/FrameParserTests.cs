using System.Buffers.Binary;
using FramedMessage.Core;

namespace FramedMessage.Tests;

public class FrameParserTests
{
    private static MemoryStream BuildStream(params (ushort len, byte[] payload)[] frames)
    {
        var ms = new MemoryStream();
        foreach (var (len, payload) in frames)
        {
            var header = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(header, len);
            ms.Write(header);
            ms.Write(payload);
        }
        ms.Position = 0;
        return ms;
    }

    [Fact]
    public void EmptyStream_ReturnsNull()
    {
        var parser = new FrameParser(new MemoryStream());
        Assert.Null(parser.ReadNext());
    }

    [Fact]
    public void SingleFrame_ReturnsPayload()
    {
        var data = "PAYMENT|123|100.00|HRK"u8.ToArray();
        var stream = BuildStream(((ushort)data.Length, data));
        Assert.Equal(data, new FrameParser(stream).ReadNext());
    }

    [Fact]
    public void MultipleFrames_ReturnedInOrder()
    {
        var a = "PAYMENT|1|100.00|HRK"u8.ToArray();
        var b = "END"u8.ToArray();
        var stream = BuildStream(((ushort)a.Length, a), ((ushort)b.Length, b));
        var parser = new FrameParser(stream);

        Assert.Equal(a, parser.ReadNext());
        Assert.Equal(b, parser.ReadNext());
        Assert.Null(parser.ReadNext());
    }

    [Fact]
    public void ZeroLength_Throws()
    {
        var stream = new MemoryStream([0x00, 0x00]);
        Assert.Throws<FrameReadingException>(() => new FrameParser(stream).ReadNext());
    }

    [Fact]
    public void TruncatedHeader_Throws()
    {
        var stream = new MemoryStream([0x00]);
        Assert.Throws<FrameReadingException>(() => new FrameParser(stream).ReadNext());
    }
}
