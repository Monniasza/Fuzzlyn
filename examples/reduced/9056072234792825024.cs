// Generated by Fuzzlyn v1.2 on 2021-08-12 07:31:34
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 9056072234792825024
// Reduced from 6.0 KiB to 0.2 KiB in 00:00:29
// Debug: Outputs -1
// Release: Outputs 0
public class Program
{
    static short s_4;
    public static void Main()
    {
        s_4 = (short)((3148768156725180696L % (s_4 | 1)) - 1);
        System.Console.WriteLine(s_4);
    }
}
