// Generated by Fuzzlyn v1.1 on 2018-08-20 10:55:57
// Seed: 3759169976523159204
// Reduced from 36.0 KiB to 0.5 KiB in 00:00:27
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public long F0;
    public short F1;
    public uint F2;
    public int F3;
    public S0(long f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    static S0 s_2;
    public static void Main()
    {
        S0 vr6 = default(S0);
        var vr4 = new S0(1);
        var vr5 = new S0(0);
        M1(vr6, M1(vr4, vr5));
    }

    static ref S0 M1(S0 arg0, S0 arg1)
    {
        System.Console.WriteLine(arg0.F0);
        return ref s_2;
    }
}
