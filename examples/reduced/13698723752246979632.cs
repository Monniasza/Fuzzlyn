// Generated by Fuzzlyn v1.1 on 2018-09-10 21:42:36
// Seed: 13698723752246979632
// Reduced from 135.7 KiB to 0.5 KiB in 00:03:22
// Debug: Outputs -14
// Release: Outputs 2
public class Program
{
    static ushort s_10;
    static bool[] s_19 = new bool[]{true};
    static sbyte[] s_38 = new sbyte[]{0};
    public static void Main()
    {
        s_10 = 39054;
        var vr10 = s_38[0];
        M46(vr10);
    }

    static void M46(sbyte arg0)
    {
        arg0 += (sbyte)s_10;
        arg0 %= -20;
        System.Console.WriteLine(arg0);
        try
        {
            bool vr1 = s_19[0];
        }
        finally
        {
            long var34 = arg0--;
        }
    }
}
