// Generated by Fuzzlyn v1.2 on 2021-08-06 15:52:20
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 1185784890734309949
// Reduced from 145.2 KiB to 0.2 KiB in 00:03:50
// Debug: Outputs -1
// Release: Outputs 0
public class Program
{
    static uint[][] s_8 = new uint[][]{new uint[]{0}};
    public static void Main()
    {
        var vr1 = (-77664846126774664L % (s_8[0][0] | 1)) - 1;
        System.Console.WriteLine(vr1);
    }
}
