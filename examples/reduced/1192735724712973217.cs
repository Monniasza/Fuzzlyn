// Generated by Fuzzlyn v1.1 on 2018-08-27 22:12:07
// Seed: 1192735724712973217
// Reduced from 4.2 KiB to 0.4 KiB
// Debug: Outputs 32768
// Release: Outputs 4294934528
public class Program
{
    static short s_4 = 32766;
    static long[] s_6 = new long[]{0};
    public static void Main()
    {
        s_6[0] = s_4++;
        s_6[0] = s_4++;
        var vr21 = (uint)((uint)s_4 ^ 0L);
        M3(vr21, s_4);
    }

    static ulong[] M3(uint arg0, short arg1)
    {
        System.Console.WriteLine(arg0);
        return new ulong[]{0};
    }
}
