// Generated by Fuzzlyn v1.1 on 2018-10-01 14:23:03
// Seed: 11732363509126071829
// Reduced from 308.8 KiB to 0.7 KiB in 00:08:31
// Debug: Prints 0 line(s)
// Release: Prints 1 line(s)
struct S0
{
    public short F0;
    public bool F1;
    public S0(bool f1): this()
    {
        F1 = f1;
    }
}

struct S1
{
    public S0 F0;
}

public class Program
{
    static ulong s_12;
    static ulong s_31;
    public static void Main()
    {
        var vr9 = new S1[]{new S1()};
        byte vr10 = (byte)M59(vr9);
    }

    static ref ulong M59(S1[] arg0)
    {
        uint var2 = default(uint);
        if (!arg0[0].F0.F1)
        {
            arg0[0].F0 = new S0(true);
            if (arg0[0].F0.F1)
            {
                return ref s_31;
            }
        }

        System.Console.WriteLine(var2);
        return ref s_12;
    }
}
