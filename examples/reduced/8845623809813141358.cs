// Generated by Fuzzlyn v1.1 on 2018-11-25 16:27:03
// Seed: 8845623809813141358
// Reduced from 117.8 KiB to 0.9 KiB in 00:02:41
// Debug: Outputs 0
// Release: Outputs 4
public class Program
{
    static long[] s_1 = new long[]{0};
    static ushort s_3;
    static uint s_8;
    static ulong s_11;
    static short[][] s_48 = new short[][]{new short[]{-1}};
    public static void Main()
    {
        var vr3 = s_48[0][0];
        bool vr4 = 0 != M3(s_3, vr3);
    }

    static long M3(ushort arg2, short arg3)
    {
        try
        {
            var vr1 = s_1[0];
        }
        finally
        {
            arg3 = (short)s_11;
            ushort[] var2 = new ushort[]{0};
            arg3 /= -32767;
            try
            {
                arg2 = var2[0];
            }
            finally
            {
                M17();
            }

            System.Console.WriteLine(arg3);
        }

        return 0;
    }

    static ref uint M17()
    {
        return ref s_8;
    }
}
