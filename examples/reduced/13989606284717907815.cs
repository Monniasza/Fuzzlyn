// Generated by Fuzzlyn on 2018-06-23 06:25:29
// Seed: 13989606284717907815
// Reduced from 82.5 KiB to 0.2 KiB
// Debug: Outputs 254
// Release: Outputs -2
public class Program
{
    static sbyte s_3;
    public static void Main()
    {
        var vr229 = s_3--;
        byte vr268 = (byte)(s_3 ^ 1U);
        short vr298 = vr268;
        System.Console.WriteLine(vr298);
    }
}
