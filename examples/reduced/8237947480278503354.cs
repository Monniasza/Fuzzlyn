// Generated by Fuzzlyn v1.1 on 2018-10-05 20:51:24
// Seed: 8237947480278503354
// Reduced from 65.9 KiB to 0.9 KiB in 00:01:35
// Debug: Outputs 0
// Release: Outputs 252
public class Program
{
    static sbyte s_4;
    static ushort s_8;
    static uint s_9;
    static ushort s_11;
    static bool s_12;
    static byte[][] s_14 = new byte[][]{new byte[]{0}};
    public static void Main()
    {
        var vr10 = new sbyte[]{-1};
        var vr13 = vr10[0];
        var vr14 = M23(vr13);
    }

    static uint M23(sbyte arg1)
    {
        try
        {
            System.Console.WriteLine(-7499424250517844623L);
        }
        finally
        {
            arg1 = s_4;
            var vr5 = s_9++ & s_8--;
            ushort vr26 = s_11++;
            byte[] var3 = s_14[0];
            ulong var4 = (ulong)(arg1 % 126);
            System.Console.WriteLine(s_12);
            System.Console.WriteLine(var4);
        }

        return M24(arg1);
    }

    static ushort M24(sbyte arg0)
    {
        byte[][] var0 = s_14;
        return s_11++;
    }
}
