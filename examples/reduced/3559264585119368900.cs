// Generated by Fuzzlyn v1.2 on 2021-07-06 08:18:41
// Seed: 3559264585119368900
// Reduced from 24.9 KiB to 0.6 KiB in 00:00:14
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public sbyte F0;
    public sbyte F1;
    public int F2;
    public short F4;
    public S0(int f2): this()
    {
        F2 = f2;
    }
}

struct S2
{
    public byte F0;
    public S0 F1;
    public S2(byte f0, S0 f1): this()
    {
        F0 = f0;
        F1 = f1;
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
        S3 vr2 = new S3(new S2(1, new S0(0)));
        System.Console.WriteLine(vr2.F0.F0);
        System.Console.WriteLine(vr2.F0.F1.F0);
    }
}
