namespace FramedMessage.Core;

public class FrameReadingException : Exception
{
    public FrameReadingException(string message) : base(message)
    {
    }
}