// Generated by Fuzzlyn v1.2 on 2021-07-04 06:03:55
// Seed: 14855418460333560153
// Reduced from 5.9 KiB to 0.6 KiB in 00:00:11
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public byte F0;
    public long F1;
    public short F5;
    public long F7;
    public sbyte F8;
    public S0(short f5): this()
    {
        F5 = f5;
    }
}

struct S1
{
    public int F0;
    public S0 F1;
    public S1(S0 f1): this()
    {
        F1 = f1;
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
    public static void Main()
    {
        S0 vr0 = new S0(1);
        S2 vr1 = new S2(new S1(new S0(1)));
        System.Console.WriteLine(vr0.F8);
        System.Console.WriteLine(vr1.F0.F1.F5);
    }
}
