// Generated by Fuzzlyn on 2018-07-08 08:55:19
// Seed: 5314589148946206089
// Reduced from 284.0 KiB to 0.2 KiB
// Debug: Runs successfully
// Release: Throws 'System.NullReferenceException'
public class Program
{
    static ushort[, ] s_1 = new ushort[, ]{{0}};
    static int s_2;
    static ushort s_3;
    public static void Main()
    {
        bool vr20 = (s_2 / ((0 & s_1[0, 0]) | 1)) == 0;
        s_3 = s_1[0, 0];
    }
}
