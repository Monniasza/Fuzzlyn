// Generated by Fuzzlyn v1.1 on 2018-12-22 08:07:15
// Seed: 8734039866128184475
// Reduced from 402.0 KiB to 0.5 KiB in 00:06:39
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public bool F1;
    public ulong F6;
}

struct S1
{
    public S0 F2;
    public sbyte F5;
    public S1(sbyte f5): this()
    {
        F5 = f5;
    }
}

public class Program
{
    static S1 s_34;
    public static void Main()
    {
        var vr3 = new S1(0);
        var vr4 = new S1(1);
        var vr5 = new S1(0);
        M57(vr3, M57(vr4, vr5));
    }

    static ref S1 M57(S1 arg0, S1 arg1)
    {
        System.Console.WriteLine(arg0.F5);
        return ref s_34;
    }
}
