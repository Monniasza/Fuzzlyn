// Generated by Fuzzlyn v1.1 on 2018-09-14 20:58:29
// Seed: 13819503370460356907
// Reduced from 26.9 KiB to 0.5 KiB in 00:00:29
// Debug: Outputs 0
// Release: Outputs 4
public class Program
{
    static ulong s_8;
    public static void Main()
    {
        ulong vr4 = default(ulong);
        var vr2 = (sbyte)vr4;
        bool vr3 = 0 < M10(vr2);
        System.Console.WriteLine(s_8);
    }

    static ushort M10(sbyte arg0)
    {
        try
        {
            int[] var0 = new int[]{0};
        }
        finally
        {
            s_8 = (ulong)(arg0-- ^ (arg0 / 62));
        }

        return 1;
    }
}
