// Generated by Fuzzlyn v1.2 on 2021-07-04 23:24:43
// Seed: 6190061698267731775
// Reduced from 612.8 KiB to 0.5 KiB in 00:10:01
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public byte F4;
}

struct S2
{
    public S0 F0;
}

struct S3
{
    public byte F1;
    public S2 F3;
    public S3(byte f1, S2 f3): this()
    {
        F1 = f1;
        F3 = f3;
    }
}

struct S5
{
    public S3 F0;
    public S5(S3 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    public static void Main()
    {
        S5 vr4 = new S5(new S3(1, new S2()));
        System.Console.WriteLine(vr4.F0.F1);
        System.Console.WriteLine(vr4.F0.F3.F0.F4);
    }
}
