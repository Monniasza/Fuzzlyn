// Generated by Fuzzlyn v1.1 on 2021-06-12 11:06:35
// Seed: 8276490119048877745
// Reduced from 962.5 KiB to 0.6 KiB in 00:05:32
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public ushort F1;
    public uint F2;
    public byte F3;
    public int F5;
    public S0(byte f3): this()
    {
        F3 = f3;
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
        S2 vr0 = new S2(new S1(new S0(1)));
        System.Console.WriteLine(vr0.F0.F0.F3);
        System.Console.WriteLine(vr0.F0.F4.F1);
    }
}
