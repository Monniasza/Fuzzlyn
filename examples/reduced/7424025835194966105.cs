// Generated by Fuzzlyn v1.2 on 2021-07-06 23:46:03
// Seed: 7424025835194966105
// Reduced from 27.6 KiB to 0.6 KiB in 00:00:13
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public short F0;
    public ulong F1;
    public sbyte F2;
    public byte F6;
    public S0(short f0): this()
    {
        F0 = f0;
    }
}

struct S1
{
    public sbyte F0;
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
        S2 vr0 = new S2(new S1(new S0(1)));
        System.Console.WriteLine(vr0.F0.F0);
        System.Console.WriteLine(vr0.F0.F1.F0);
    }
}
