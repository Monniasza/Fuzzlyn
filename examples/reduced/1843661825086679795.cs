// Generated by Fuzzlyn v1.2 on 2021-07-21 15:28:57
// Seed: 1843661825086679795
// Reduced from 413.4 KiB to 0.6 KiB in 00:03:14
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public int F1;
    public ushort F2;
    public short F5;
    public short F6;
    public S0(int f1): this()
    {
        F1 = f1;
    }
}

struct S1
{
    public ushort F0;
    public uint F4;
    public S0 F5;
    public S1(ushort f0, S0 f5): this()
    {
        F0 = f0;
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
        S2 vr0 = new S2(new S1(1, new S0(1)));
        System.Console.WriteLine(vr0.F0.F0);
        System.Console.WriteLine(vr0.F0.F4);
    }
}
