// Generated by Fuzzlyn on 2018-07-24 10:21:06
// Seed: 1862355168484221712
// Reduced from 34.0 KiB to 0.2 KiB
// Debug: Outputs 65506
// Release: Outputs -30
public class Program
{
    static sbyte s_1;
    public static void Main()
    {
        s_1 = -30;
        char vr11 = (char)(s_1 ^ (uint)0);
        System.Console.WriteLine((int)vr11);
    }
}
