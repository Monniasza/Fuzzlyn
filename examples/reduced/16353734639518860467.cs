// Generated by Fuzzlyn v1.2 on 2021-07-22 03:08:49
// Seed: 16353734639518860467
// Reduced from 133.3 KiB to 0.6 KiB in 00:01:20
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public sbyte F0;
    public ushort F1;
    public ulong F2;
    public ulong F3;
    public S0(sbyte f0): this()
    {
        F0 = f0;
    }
}

struct S2
{
    public S0 F0;
    public bool F4;
    public S2(S0 f0): this()
    {
        F0 = f0;
    }
}

struct S6
{
    public S2 F0;
    public S6(S2 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    public static void Main()
    {
        S6 vr9 = new S6(new S2(new S0(1)));
        System.Console.WriteLine(vr9.F0.F0.F0);
        System.Console.WriteLine(vr9.F0.F0.F1);
    }
}
