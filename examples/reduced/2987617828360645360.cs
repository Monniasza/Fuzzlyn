// Generated by Fuzzlyn v1.1 on 2018-10-01 05:06:07
// Seed: 2987617828360645360
// Reduced from 211.9 KiB to 1.0 KiB in 00:03:35
// Debug: Outputs 0
// Release: Outputs -2
public class Program
{
    static bool[] s_3 = new bool[]{false};
    static uint s_10;
    static sbyte s_42;
    static ulong s_45;
    static ulong[, ] s_98;
    static uint s_109;
    public static void Main()
    {
        M72(-1);
    }

    static void M72(sbyte arg0)
    {
        int var2 = default(int);
        arg0 = s_42;
        arg0 %= (-128 | 1);
        try
        {
            System.Console.WriteLine(0);
        }
        finally
        {
            s_109 = s_10--;
            if (s_3[0])
            {
                if ((0 < s_45))
                {
                    M82();
                }

                int var29 = var2;
                System.Console.WriteLine(var29);
            }

            M78() = new ulong[, ]{{0, 0, 0, 0, 0, 0, 1}};
            var2 %= 1;
        }

        System.Console.WriteLine(arg0);
    }

    static ref ulong[, ] M78()
    {
        return ref s_98;
    }

    static byte M82()
    {
        return default(byte);
    }
}
