// Generated by Fuzzlyn v1.1 on 2018-08-30 05:37:37
// Seed: 2686126150750747046
// Reduced from 192.8 KiB to 0.6 KiB in 00:03:49
// Debug: Outputs 0
// Release: Outputs -16711680
public class Program
{
    static short s_26 = -1;
    public static void Main()
    {
        M5(s_26);
    }

    static void M5(short arg2)
    {
        var vr1 = new long[][, ]{new long[, ]{{0}}};
        arg2 = (short)M30(vr1);
        try
        {
            arg2 |= 0;
            var vr0 = 255 * arg2;
            System.Console.WriteLine(vr0);
        }
        finally
        {
            --arg2;
        }
    }

    static int M30(long[][, ] arg1)
    {
        arg1[0][0, 0] = arg1[0][0, 0];
        return 0;
    }
}
