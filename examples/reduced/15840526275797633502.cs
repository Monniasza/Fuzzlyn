// Generated by Fuzzlyn v1.1 on 2018-09-21 22:45:55
// Seed: 15840526275797633502
// Reduced from 142.9 KiB to 0.7 KiB in 00:01:40
// Debug: Outputs 0
// Release: Outputs 1
class C0
{
    public ulong F0;
    public long F2;
    public C0(ulong f0)
    {
        F0 = f0;
    }
}

struct S0
{
    public C0 F3;
    public S0(C0 f3): this()
    {
        F3 = f3;
    }
}

struct S1
{
    public S0 F0;
    public C0 F3;
    public long F5;
    public S1(S0 f0, C0 f3): this()
    {
        F0 = f0;
        F3 = f3;
    }
}

public class Program
{
    public static void Main()
    {
        S1[] vr0 = new S1[]{new S1(new S0(new C0(0)), new C0(0))};
        vr0[0].F5 = vr0[0].F0.F3.F2;
        vr0[0].F0.F3 = new C0(1);
        vr0[0].F0.F3.F0 = vr0[0].F3.F0;
        System.Console.WriteLine(vr0[0].F0.F3.F0);
    }
}
