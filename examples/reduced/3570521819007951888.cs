// Generated by Fuzzlyn v1.2 on 2021-08-17 13:16:10
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 3570521819007951888
// Reduced from 83.2 KiB to 0.2 KiB in 00:02:37
// Debug: Outputs 1
// Release: Outputs 0
public class Program
{
    static sbyte s_13;
    public static void Main()
    {
        var vr2 = new long[]{1};
        s_13 = (sbyte)((2664097651610060831L % vr2[0]) + 1);
        System.Console.WriteLine(s_13);
    }
}
