// Generated by Fuzzlyn v1.2 on 2021-07-06 00:16:02
// Seed: 4547719367665829101
// Reduced from 143.3 KiB to 0.6 KiB in 00:01:13
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public ulong F0;
    public long F1;
    public int F2;
    public sbyte F3;
    public S0(int f2): this()
    {
        F2 = f2;
    }
}

struct S1
{
    public ulong F1;
    public S0 F2;
    public S1(ulong f1, S0 f2): this()
    {
        F1 = f1;
        F2 = f2;
    }
}

struct S2
{
    public S1 F0;
    public S2(S1 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    static S2 s_12;
    public static void Main()
    {
        S2 vr9 = new S2(new S1(1, new S0(0)));
        s_12.F0.F2.F0 = vr9.F0.F2.F0;
        System.Console.WriteLine(vr9.F0.F1);
    }
}
