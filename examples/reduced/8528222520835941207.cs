// Generated by Fuzzlyn v1.2 on 2021-08-07 11:22:50
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 8528222520835941207
// Reduced from 261.2 KiB to 0.6 KiB in 00:07:06
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public sbyte F0;
    public long F1;
    public long F5;
    public bool F8;
    public S0(long f5): this()
    {
        F5 = f5;
    }
}

struct S1
{
    public short F0;
    public S0 F1;
    public S1(short f0, S0 f1): this()
    {
        F0 = f0;
        F1 = f1;
    }
}

struct S4
{
    public S1 F0;
    public S4(S1 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    public static void Main()
    {
        S4 vr2 = new S4(new S1(1, new S0(0)));
        System.Console.WriteLine(vr2.F0.F0);
        System.Console.WriteLine(vr2.F0.F1.F0);
    }
}
