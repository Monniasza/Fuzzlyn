// Generated by Fuzzlyn v1.1 on 2018-12-24 20:08:15
// Seed: 1994560479106976020
// Reduced from 95.3 KiB to 0.5 KiB in 00:00:59
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public uint F0;
    public long F3;
}

struct S2
{
    public long F0;
    public S0 F3;
    public S2(long f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    static S2 s_17;
    public static void Main()
    {
        var vr3 = new S2(0);
        var vr4 = new S2(1);
        var vr5 = new S2(0);
        M24(vr3, M24(vr4, vr5));
    }

    static ref S2 M24(S2 arg0, S2 arg1)
    {
        System.Console.WriteLine(arg0.F0);
        return ref s_17;
    }
}
