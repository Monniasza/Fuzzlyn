// Generated by Fuzzlyn v1.3 on 2021-08-23 08:28:10
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 6389032397173806698
// Reduced from 94.8 KiB to 0.6 KiB in 00:01:15
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public int F0;
    public uint F2;
    public uint F6;
    public bool F7;
    public S0(int f0): this()
    {
        F0 = f0;
    }
}

struct S1
{
    public bool F0;
    public sbyte F2;
    public S0 F5;
    public S1(sbyte f2, S0 f5): this()
    {
        F2 = f2;
        F5 = f5;
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
        S2 vr0 = new S2(new S1(1, new S0(0)));
        System.Console.WriteLine(vr0.F0.F0);
        System.Console.WriteLine(vr0.F0.F2);
    }
}
