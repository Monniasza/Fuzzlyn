// Generated by Fuzzlyn v1.2 on 2021-08-15 23:15:19
// Run on .NET 6.0.0-dev on Arm Linux
// Seed: 18219619158927602726
// Reduced from 82.6 KiB to 0.3 KiB in 00:02:54
// Debug: Outputs 14270
// Release: Outputs 4294953026
public class Program
{
    static long[] s_28 = new long[]{1};
    public static void Main()
    {
        var vr10 = s_28[0];
        for (int vr13 = 0; vr13 < 2; vr13++)
        {
            uint vr12 = (uint)(0 - (-14270 * vr10));
            System.Console.WriteLine(vr12);
        }
    }
}
