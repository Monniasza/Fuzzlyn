// Generated by Fuzzlyn v1.1 on 2018-12-09 11:21:56
// Seed: 12119026237819808780
// Reduced from 6.2 KiB to 0.4 KiB in 00:00:14
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public byte F2;
    public ulong F4;
    public int F7;
    public S0(byte f2): this()
    {
        F2 = f2;
    }
}

public class Program
{
    static S0 s_2;
    public static void Main()
    {
        var vr2 = new S0(1);
        var vr3 = new S0(0);
        M1(vr2, M1(s_2, vr3));
    }

    static ref S0 M1(S0 arg0, S0 arg1)
    {
        System.Console.WriteLine(arg0.F2);
        return ref s_2;
    }
}
