// Generated by Fuzzlyn v1.2 on 2021-08-06 12:34:39
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 4050368954844172126
// Reduced from 79.7 KiB to 0.2 KiB in 00:01:46
// Debug: Outputs 65535
// Release: Outputs 0
public class Program
{
    static ushort s_7;
    public static void Main()
    {
        s_7 = (ushort)((-388739530 % (s_7 | 1)) - 1);
        System.Console.WriteLine(s_7);
    }
}
