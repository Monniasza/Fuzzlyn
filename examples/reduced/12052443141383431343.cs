// Generated by Fuzzlyn v1.2 on 2021-07-22 13:36:33
// Seed: 12052443141383431343
// Reduced from 53.8 KiB to 0.4 KiB in 00:00:33
// Debug: Outputs 0
// Release: Outputs 245
struct S0
{
    public byte F0;
    public long F1;
    public ulong F3;
    public uint F5;
    public sbyte F7;
    public S0(uint f5): this()
    {
        F5 = f5;
    }
}

public class Program
{
    static S0 s_11 = new S0(261813483U);
    public static void Main()
    {
        S0 vr2 = default(S0);
        vr2.F5 = s_11.F5 / 13464;
        System.Console.WriteLine(vr2.F0);
    }
}
