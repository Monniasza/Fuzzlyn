// Generated by Fuzzlyn v1.1 on 2018-09-30 21:59:30
// Seed: 3928658557286920331
// Reduced from 342.5 KiB to 0.8 KiB in 00:06:02
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public long F0;
    public ulong F3;
    public S0(long f0): this()
    {
        F0 = f0;
    }
}

class C2
{
    public ulong F2;
}

struct S1
{
    public S0 F1;
    public C2 F5;
    public S1(S0 f1, C2 f5): this()
    {
        F1 = f1;
        F5 = f5;
    }
}

struct S2
{
    public S1 F2;
    public sbyte F4;
    public S2(S1 f2): this()
    {
        F2 = f2;
    }
}

public class Program
{
    static S1 s_21;
    static S2[] s_30 = new S2[]{new S2(new S1(new S0(1), new C2()))};
    public static void Main()
    {
        S2[] vr0 = s_30;
        vr0[0].F2.F5.F2 = s_21.F1.F3;
        vr0[0].F2.F5 = new C2();
        vr0[0].F2.F5.F2 = (ulong)vr0[0].F2.F1.F0;
        System.Console.WriteLine(vr0[0].F2.F5.F2);
    }
}
