// Generated by Fuzzlyn v1.2 on 2021-08-15 23:18:20
// Run on .NET 6.0.0-dev on Arm Linux
// Seed: 18169784530849023822
// Reduced from 153.7 KiB to 0.7 KiB in 00:03:01
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public byte F0;
    public ulong F1;
    public short F2;
    public sbyte F3;
    public S0(short f2): this()
    {
        F2 = f2;
    }
}

struct S2
{
    public sbyte F0;
    public S0 F6;
    public S2(sbyte f0, S0 f6): this()
    {
        F0 = f0;
        F6 = f6;
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
    static S0[][] s_1 = new S0[][]{new S0[]{new S0(0)}};
    public static void Main()
    {
        S3 vr0 = new S3(new S2(1, new S0(0)));
        s_1[0][0].F0 = vr0.F0.F6.F0;
        System.Console.WriteLine(vr0.F0.F0);
    }
}
