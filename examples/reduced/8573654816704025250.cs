// Generated by Fuzzlyn on 2018-07-19 03:44:31
// Seed: 8573654816704025250
// Reduced from 26.6 KiB to 0.2 KiB
// Debug: Outputs 4294967274
// Release: Outputs 234
public class Program
{
    static sbyte s_1 = -23;
    public static void Main()
    {
        var vr32 = (uint)(s_1 - -9223372036854775808L) - -1;
        System.Console.WriteLine(vr32);
    }
}
