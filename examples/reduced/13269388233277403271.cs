// Generated by Fuzzlyn on 2018-06-25 18:08:00
// Seed: 13269388233277403271
// Reduced from 9.2 KiB to 0.3 KiB
// Debug: Outputs 65280
// Release: Outputs -65536
public class Program
{
    public static void Main()
    {
        long vr51 = 65535 ^ -2;
        long vr35 = 65535 ^ vr51;
        var vr36 = (sbyte)vr35;
        var vr41 = M3(vr36);
        System.Console.WriteLine((int)vr41);
    }

    static long M3(sbyte arg0)
    {
        ++arg0;
        return (65535 ^ (byte)(0L | arg0));
    }
}
