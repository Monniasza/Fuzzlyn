// Generated by Fuzzlyn v1.1 on 2018-10-03 11:32:09
// Seed: 15996622878912482459
// Reduced from 256.0 KiB to 0.4 KiB in 00:08:14
// Debug: Outputs 0
// Release: Outputs 803111424
struct S0
{
    public sbyte F0;
}

public class Program
{
    static S0[] s_25 = new S0[]{new S0()};
    public static void Main()
    {
        S0 vr10 = s_25[0];
        var vr11 = vr10.F0 * 24702;
        System.Console.WriteLine(vr11);
        try
        {
            System.Console.WriteLine(52497);
        }
        finally
        {
            vr10.F0 = vr10.F0;
        }
    }
}
