// Generated by Fuzzlyn v1.1 on 2018-10-03 11:40:33
// Seed: 18406191664133598668
// Reduced from 234.8 KiB to 1.0 KiB in 00:04:42
// Debug: Outputs 0
// Release: Outputs 31828
struct S0
{
    public ushort F0;
    public long F1;
    public ulong F2;
}

class C0
{
    public S0 F0;
}

public class Program
{
    static sbyte s_17;
    static uint s_22;
    public static void Main()
    {
        var vr10 = new S0();
        var vr11 = new C0[][, ]{new C0[, ]{{new C0()}}};
        ulong vr17 = default(ulong);
        var vr12 = (byte)vr17;
        var vr13 = new bool[][]{new bool[]{true}};
        long vr18 = default(long);
        var vr14 = (uint)vr18;
        var vr15 = new ushort[][]{new ushort[]{0}};
        var vr16 = M57(vr10, M64(vr11, vr12, vr13, s_17, vr14, vr15, ref s_22));
    }

    static long M57(S0 arg0, S0 arg1)
    {
        System.Console.WriteLine(arg0.F0);
        return arg0.F1;
    }

    static ref S0 M64(C0[][, ] arg0, byte arg1, bool[][] arg3, sbyte arg4, uint arg5, ushort[][] arg6, ref uint arg7)
    {
        arg0[0][0, 0] = new C0();
        return ref arg0[0][0, 0].F0;
    }
}
