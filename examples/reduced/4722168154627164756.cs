// Generated by Fuzzlyn v1.1 on 2018-11-25 07:44:14
// Seed: 4722168154627164756
// Reduced from 440.7 KiB to 0.6 KiB in 00:06:52
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public ulong F0;
    public S0(ulong f0): this()
    {
        F0 = f0;
    }
}

class C0
{
}

struct S1
{
    public S0 F0;
    public sbyte F3;
    public C0 F5;
    public S1(S0 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    static S1 s_12;
    public static void Main()
    {
        var vr8 = new S1(new S0(1));
        var vr9 = new S1(new S0(0));
        M27(s_12, M27(vr8, vr9));
    }

    static ref S1 M27(S1 arg0, S1 arg1)
    {
        System.Console.WriteLine(arg0.F0.F0);
        return ref s_12;
    }
}
