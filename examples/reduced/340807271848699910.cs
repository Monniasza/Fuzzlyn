// Generated by Fuzzlyn v1.1 on 2018-11-28 21:35:33
// Seed: 340807271848699910
// Reduced from 64.4 KiB to 0.8 KiB in 00:00:53
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public ushort F0;
    public uint F1;
    public int F2;
    public byte F3;
    public uint F4;
    public S0(ushort f0): this()
    {
        F0 = f0;
    }
}

struct S1
{
    public int F1;
    public ulong F3;
    public short F4;
}

struct S2
{
    public ushort F0;
}

class C1
{
    public S1 F1;
    public S2 F6;
}

public class Program
{
    static C1 s_4 = new C1();
    static S0[] s_9 = new S0[]{new S0(0)};
    public static void Main()
    {
        var vr2 = new S0(1);
        S1 vr4 = s_4.F1;
        M16(vr2, M15(ref s_4.F6, vr4));
    }

    static ref S0 M15(ref S2 arg0, S1 arg2)
    {
        System.Console.WriteLine(arg0.F0);
        return ref s_9[0];
    }

    static void M16(S0 arg0, S0 arg1)
    {
        System.Console.WriteLine(arg0.F0);
    }
}
