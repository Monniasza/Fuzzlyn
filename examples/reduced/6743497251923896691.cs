// Generated by Fuzzlyn v1.2 on 2021-08-12 08:04:55
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 6743497251923896691
// Reduced from 235.9 KiB to 0.2 KiB in 00:05:54
// Debug: Outputs 1
// Release: Outputs 0
public class Program
{
    public static void Main()
    {
        byte[] vr2 = new byte[]{1};
        var vr3 = (1432888208322567936UL % vr2[0]) + 1;
        System.Console.WriteLine(vr3);
    }
}
