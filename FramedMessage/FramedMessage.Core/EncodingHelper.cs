using System.Text;

namespace FramedMessage.Core;

public static class EncodingHelper
{
    private static readonly Encoding IsoEncoding;

    static EncodingHelper()
    {
        // not included in .NET's default set
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        IsoEncoding = Encoding.GetEncoding("iso-8859-2");
    }

    public static string DecodeIso88592(byte[] payload)
    {
        return IsoEncoding.GetString(payload);
    }

    public static byte[] EncodeIso88592(string text)
    {
        return IsoEncoding.GetBytes(text);
    }
}