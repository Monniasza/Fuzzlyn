// Generated by Fuzzlyn v1.2 on 2021-08-11 17:14:39
// Run on .NET 6.0.0-dev on X64 Windows
// Seed: 3298236103913364758
// Reduced from 130.2 KiB to 0.6 KiB in 00:00:56
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public long F0;
    public uint F1;
    public long F3;
    public sbyte F4;
    public S0(long f0): this()
    {
        F0 = f0;
    }
}

struct S1
{
    public bool F0;
    public ushort F1;
    public S0 F9;
    public S1(bool f0, S0 f9): this()
    {
        F0 = f0;
        F9 = f9;
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
        S2 vr2 = new S2(new S1(true, new S0(0)));
        System.Console.WriteLine(vr2.F0.F0);
        System.Console.WriteLine(vr2.F0.F1);
    }
}
