// Generated by Fuzzlyn v1.1 on 2018-09-18 12:50:43
// Seed: 13172325245170110222
// Reduced from 269.6 KiB to 0.8 KiB in 00:08:07
// Debug: Outputs 0
// Release: Outputs 7274496
struct S0
{
    public uint F2;
}

struct S1
{
    public short F5;
    public S1(short f5): this()
    {
        F5 = f5;
    }
}

public class Program
{
    static S1 s_25 = new S1(-1);
    static S0 s_75;
    public static void Main()
    {
        var vr5 = s_25.F5;
        M61(vr5);
    }

    static void M61(short arg0)
    {
        try
        {
            M79();
        }
        finally
        {
            short vr7 = default(short);
            arg0 <<= vr7;
            arg0 = (short)M82();
            var vr2 = arg0 * -111;
            System.Console.WriteLine(vr2);
        }
    }

    static sbyte M79()
    {
        return default(sbyte);
    }

    static int M82()
    {
        var vr1 = s_75.F2;
        return 0;
    }
}
