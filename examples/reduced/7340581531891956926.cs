// Generated by Fuzzlyn v1.2 on 2021-08-11 16:27:57
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 7340581531891956926
// Reduced from 231.1 KiB to 0.4 KiB in 00:08:38
// Debug: Outputs 18446744073709551615
// Release: Outputs 0
public class Program
{
    static uint[][] s_103 = new uint[][]{new uint[]{0}};
    public static void Main()
    {
        short vr2 = M104();
        var vr1 = (17570677317119195126UL % ((ulong)vr2 | 1)) - 1;
        System.Console.WriteLine(vr1);
    }

    static byte M104()
    {
        s_103[0] = new uint[]{0, 1};
        return 0;
    }
}
