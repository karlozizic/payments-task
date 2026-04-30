using System.Buffers.Binary;

namespace FramedMessage.Core;

public class FrameParser
{
    private readonly Stream _stream;
    
    public FrameParser(Stream stream)
    {
        _stream = stream;
    }

    public byte[]? ReadNext()
    {
        var header = new byte[2];
        int read = FillBuffer(header, 2);

        if (read == 0)
            return null;
        
        if (read < 2)
            throw new FrameReadingException("Header is incomplete");
        
        ushort payloadLength = BinaryPrimitives.ReadUInt16BigEndian(header);
        
        if (payloadLength == 0)
            throw new FrameReadingException("Payload length cannot be zero");
        
        if (payloadLength > 4096)
            throw new FrameReadingException($"Payload length {payloadLength} exceeds maximum allowed size");
        
        var payload = new byte[payloadLength];
        int payloadRead = FillBuffer(payload, payloadLength);
        
        if (payloadRead < payloadLength)
            throw new FrameReadingException("Payload is incomplete");
        
        return payload;
    }
    
    private int FillBuffer(byte[] buffer, int count)
    {
        int total = 0;
        while (total < count)
        {
            // TCP may deliver fewer bytes than requested
            int n = _stream.Read(buffer, total, count - total);
            if (n == 0) break;
            total += n;
        }
        return total;
    }
}