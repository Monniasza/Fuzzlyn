// Generated by Fuzzlyn v1.2 on 2021-07-22 13:43:51
// Seed: 5718648838179817730
// Reduced from 250.3 KiB to 0.5 KiB in 00:03:27
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public long F0;
    public uint F1;
    public byte F3;
    public int F4;
    public int F7;
    public S0(long f0, uint f1): this()
    {
        F0 = f0;
        F1 = f1;
    }
}

struct S1
{
    public S0 F5;
}

public class Program
{
    static S1 s_5;
    static uint s_19;
    public static void Main()
    {
        S0 vr1;
        s_5.F5 = new S0(1, s_19-- / 29);
        vr1.F0 = s_5.F5.F0;
        System.Console.WriteLine(vr1.F0);
    }
}
