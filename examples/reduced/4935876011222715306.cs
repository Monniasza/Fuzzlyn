// Generated by Fuzzlyn v1.1 on 2018-12-27 00:12:57
// Seed: 4935876011222715306
// Reduced from 486.4 KiB to 0.8 KiB in 00:10:34
// Debug: Outputs 0
// Release: Outputs -4392
struct S0
{
    public ulong F0;
    public long F2;
    public byte F5;
}

struct S1
{
    public bool F0;
    public long F3;
}

struct S2
{
    public short F0;
    public S1 F2;
}

class C0
{
}

struct S3
{
    public ushort F1;
    public long F4;
    public C0 F6;
    public S3(C0 f6): this()
    {
        F6 = f6;
    }
}

public class Program
{
    static S0 s_13;
    static S0 s_57;
    public static void Main()
    {
        var vr2 = new S2();
        var vr3 = new S3(new C0());
        M82(vr2, M55(vr3));
    }

    static ref S0 M55(S3 arg1)
    {
        S0 var8 = s_13;
        return ref s_57;
    }

    static void M82(S2 arg0, S0 arg1)
    {
        System.Console.WriteLine(arg0.F0);
    }
}
