// Generated by Fuzzlyn v1.1 on 2018-09-03 11:47:14
// Seed: 1994571092237444288
// Reduced from 421.5 KiB to 1.1 KiB in 00:18:34
// Debug: Outputs 0
// Release: Outputs 1909006848
class C0
{
    public int F0;
    public uint F1;
    public C0(int f0, uint f1)
    {
        F0 = f0;
        F1 = f1;
    }
}

public class Program
{
    static sbyte s_31 = 1;
    static byte s_80;
    static int[] s_87;
    public static void Main()
    {
        M99(-1);
    }

    static void M99(sbyte arg1)
    {
        ulong var4 = default(ulong);
        if (arg1 != 1)
        {
            arg1 %= s_31;
            var vr14 = (int)(3901634270U * arg1);
            System.Console.WriteLine(vr14);
        }
        else
        {
            try
            {
                System.Console.WriteLine(s_80);
            }
            finally
            {
                var vr4 = s_87[0];
                var vr7 = s_87[0]--;
                var vr10 = new C0(0, 0);
                M101();
                var vr11 = new C0(-1, 0);
                M101();
                System.Console.WriteLine(var4);
            }
        }

        System.Console.WriteLine(arg1);
    }

    static short M101()
    {
        return default(short);
    }
}
