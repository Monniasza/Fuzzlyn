// Generated by Fuzzlyn v1.2 on 2021-08-11 10:42:47
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 4664278260634755324
// Reduced from 114.8 KiB to 0.2 KiB in 00:04:27
// Debug: Outputs 1
// Release: Outputs 0
public class Program
{
    static int s_2;
    static long s_38;
    public static void Main()
    {
        s_2 = (ushort)((14862277673855554112UL % (byte)(s_38 | 1)) + 1);
        System.Console.WriteLine(s_2);
    }
}
