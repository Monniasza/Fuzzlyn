// Generated by Fuzzlyn v1.2 on 2021-07-05 22:18:01
// Seed: 2490745386890520526
// Reduced from 343.9 KiB to 0.6 KiB in 00:02:31
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public ushort F0;
    public uint F2;
    public byte F4;
    public short F6;
    public S0(uint f2): this()
    {
        F2 = f2;
    }
}

struct S1
{
    public S0 F0;
    public S0 F4;
    public S1(S0 f0): this()
    {
        F0 = f0;
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
        S2 vr2 = new S2(new S1(new S0(1)));
        System.Console.WriteLine(vr2.F0.F0.F0);
        System.Console.WriteLine(vr2.F0.F0.F2);
    }
}
