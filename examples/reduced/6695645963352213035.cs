// Generated by Fuzzlyn v1.2 on 2021-07-04 03:43:05
// Seed: 6695645963352213035
// Reduced from 111.0 KiB to 0.6 KiB in 00:00:37
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public sbyte F1;
    public bool F3;
    public int F5;
    public bool F7;
    public S0(int f5): this()
    {
        F5 = f5;
    }
}

struct S1
{
    public ushort F0;
    public S0 F1;
    public S0 F6;
    public S1(ushort f0, S0 f6): this()
    {
        F0 = f0;
        F6 = f6;
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
        S2 vr2 = new S2(new S1(1, new S0(0)));
        System.Console.WriteLine(vr2.F0.F0);
        System.Console.WriteLine(vr2.F0.F1.F1);
    }
}
