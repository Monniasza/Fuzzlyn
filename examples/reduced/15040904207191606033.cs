// Generated by Fuzzlyn v1.1 on 2018-08-12 07:23:52
// Seed: 15040904207191606033
// Reduced from 5.2 KiB to 0.6 KiB
// Debug: Outputs 3870812984
// Release: Outputs 0
struct S0
{
    public uint F1;
    public S0(uint f1): this()
    {
        F1 = f1;
    }
}

struct S1
{
    public S0 F3;
    public S1(S0 f3): this()
    {
        F3 = f3;
    }
}

struct S2
{
    public long F2;
    public S1 F4;
    public S2(S1 f4): this()
    {
        F4 = f4;
    }
}

struct S3
{
    public S2 F0;
    public S3(S2 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    public static void Main()
    {
        S3 vr0 = new S3(new S2(new S1(new S0(3870812984U))));
        uint vr1 = vr0.F0.F4.F3.F1;
        System.Console.WriteLine(vr1);
    }
}
