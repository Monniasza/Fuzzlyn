// Generated by Fuzzlyn on 2018-07-21 11:05:21
// Seed: 34884612416181630
// Reduced from 15.0 KiB to 0.3 KiB
// Debug: Runs successfully
// Release: Throws 'System.NullReferenceException'
public class Program
{
    static int[][][, ] s_3 = new int[][][, ]{new int[][, ]{new int[, ]{{0}}}};
    public static void Main()
    {
        long vr11 = 0 & s_3[0][0][0, 0];
        s_3[0] = s_3[0];
        System.Console.WriteLine(vr11);
    }
}
