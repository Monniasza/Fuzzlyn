// Generated by Fuzzlyn v1.2 on 2021-07-03 21:03:57
// Seed: 15941538739852851632
// Reduced from 20.9 KiB to 0.6 KiB in 00:00:23
// Debug: Outputs True
// Release: Outputs False
struct S0
{
    public uint F1;
    public bool F2;
    public int F3;
    public ulong F5;
    public S0(bool f2): this()
    {
        F2 = f2;
    }
}

struct S1
{
    public S0 F0;
    public int F1;
    public S1(S0 f0): this()
    {
        F0 = f0;
    }
}

struct S3
{
    public S1 F0;
    public S3(S1 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    public static void Main()
    {
        S3 vr0 = new S3(new S1(new S0(true)));
        System.Console.WriteLine(vr0.F0.F0.F1);
        System.Console.WriteLine(vr0.F0.F0.F2);
    }
}
