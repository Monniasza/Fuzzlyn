// Generated by Fuzzlyn v1.2 on 2021-08-13 13:49:40
// Run on .NET 6.0.0-dev on Arm Linux
// Seed: 1178195832502633980
// Reduced from 220.5 KiB to 0.9 KiB in 00:07:03
// Debug: Outputs 0
// Release: Outputs 139808000
struct S0
{
    public uint F0;
    public byte F1;
    public uint F2;
    public S0(uint f2): this()
    {
        F2 = f2;
    }
}

struct S1
{
}

public class Program
{
    static S0 s_15;
    public static void Main()
    {
        bool vr11 = M7();
    }

    static bool M7()
    {
        S0 var0 = default(S0);
        var vr1 = new S1[]{new S1()};
        var vr2 = new S0[]{new S0(0)};
        var vr3 = new S1();
        var vr4 = new S0(139808049U);
        M13(vr1, vr2, vr3, vr4, ref var0);
        S1[] vr17 = default(S1[]);
        var vr9 = new S0[]{new S0(0)};
        var vr10 = new S1();
        M13(vr17, vr9, vr10, var0, ref s_15);
        bool vr18 = default(bool);
        return vr18;
    }

    static void M13(S1[] arg0, S0[] arg1, S1 arg2, S0 arg3, ref S0 arg9)
    {
        System.Console.WriteLine(arg3.F2);
    }
}
