// Generated by Fuzzlyn v1.2 on 2021-08-08 22:20:15
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 16334176278240431905
// Reduced from 71.0 KiB to 0.3 KiB in 00:02:50
// Debug: Outputs 1
// Release: Outputs 0
public class Program
{
    static byte[][] s_3 = new byte[][]{new byte[]{0}};
    public static void Main()
    {
        uint vr15 = (uint)((1 + (10456364286616845701UL % (byte)(s_3[0][0] | 1))) ^ (byte)(0 * s_3[0][0]));
        System.Console.WriteLine(vr15);
    }
}
