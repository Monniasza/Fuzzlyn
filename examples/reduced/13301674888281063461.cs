// Generated by Fuzzlyn v1.1 on 2018-09-12 15:10:36
// Seed: 13301674888281063461
// Reduced from 6.3 KiB to 0.7 KiB in 00:00:20
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public ulong F0;
    public byte F2;
    public uint F4;
    public bool F5;
    public S0(sbyte f1, byte f2, uint f3, uint f4, bool f5): this()
    {
        F2 = f2;
    }
}

public class Program
{
    static S0 s_4 = new S0(0, 1, 0, 0, true);
    static ushort s_6;
    public static void Main()
    {
        var vr5 = new S0(-1, 0, 0, 0, false);
        s_6 = M2(vr5, M1(M2(s_4, s_4)));
        System.Console.WriteLine(s_6);
    }

    static ref S0 M1(long arg0)
    {
        var vr2 = new S0(0, 0, 0, 0, true);
        return ref s_4;
    }

    static byte M2(S0 arg0, S0 arg1)
    {
        System.Console.WriteLine(7774372524691261053L);
        return arg0.F2;
    }
}
