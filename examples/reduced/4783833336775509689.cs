// Generated by Fuzzlyn v1.2 on 2021-07-03 17:40:13
// Seed: 4783833336775509689
// Reduced from 580.4 KiB to 0.6 KiB in 00:05:37
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public short F0;
    public short F1;
    public int F4;
    public uint F5;
    public S0(short f0): this()
    {
        F0 = f0;
    }
}

struct S4
{
    public int F0;
    public S0 F1;
    public S4(S0 f1): this()
    {
        F1 = f1;
    }
}

struct S6
{
    public S4 F0;
    public S6(S4 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    public static void Main()
    {
        S6 vr1 = new S6(new S4(new S0(1)));
        System.Console.WriteLine(vr1.F0.F1.F0);
        System.Console.WriteLine(vr1.F0.F1.F5);
    }
}
