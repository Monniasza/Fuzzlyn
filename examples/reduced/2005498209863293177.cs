// Generated by Fuzzlyn v1.2 on 2021-08-14 17:02:46
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 2005498209863293177
// Reduced from 52.4 KiB to 0.2 KiB in 00:00:53
// Debug: Outputs 1
// Release: Outputs 0
public class Program
{
    static byte s_4;
    public static void Main()
    {
        short vr0 = (short)(1 + (-1047478670 % (s_4 | 1)));
        System.Console.WriteLine(vr0);
    }
}
