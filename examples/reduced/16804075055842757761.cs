// Generated by Fuzzlyn on 2018-06-19 20:07:17
// Seed: 16804075055842757761
// Reduced from 68.8 KiB to 0.5 KiB
// Debug: Outputs 38333
// Release: Outputs 0
struct S0
{
    public ushort F0;
    public S0(ushort f0): this()
    {
        F0 = f0;
    }
}

struct S1
{
    public S0 F1;
    public S0 F2;
    public S1(S0 f2): this()
    {
        F2 = f2;
    }
}

public class Program
{
    static S1 s_2;
    static S1 s_12 = new S1(new S0(38333));
    public static void Main()
    {
        s_12.F1 = M21();
        System.Console.WriteLine(s_12.F2.F0);
    }

    static S0 M21()
    {
        return s_2.F1;
    }
}
