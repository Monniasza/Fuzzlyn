// Generated by Fuzzlyn on 2018-06-27 19:03:34
// Seed: 13744427607597860281
// Reduced from 80.2 KiB to 0.2 KiB
// Debug: Outputs 220
// Release: Outputs -36
public class Program
{
    static sbyte s_2;
    public static void Main()
    {
        s_2 = -36;
        short vr21 = (byte)(s_2 / (uint)1);
        System.Console.WriteLine(vr21);
    }
}
