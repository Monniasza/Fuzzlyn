// Generated by Fuzzlyn v1.1 on 2018-09-09 19:17:59
// Seed: 5527031711353254079
// Reduced from 123.9 KiB to 0.8 KiB in 00:03:11
// Debug: Outputs 0
// Release: Outputs -4
public class Program
{
    static sbyte s_5;
    static sbyte[] s_20;
    static ushort[] s_23 = new ushort[]{0};
    static short s_33 = -1;
    public static void Main()
    {
        short vr21 = default(short);
        M31(false, vr21);
    }

    static void M31(bool arg1, short arg3)
    {
        uint var0 = default(uint);
        long var5 = default(long);
        arg3 = s_33;
        arg3 /= -14680;
        try
        {
            M32();
        }
        finally
        {
            sbyte var6 = s_5++;
            s_20 = new sbyte[]{1};
            var6 = var6;
            arg1 = var0++ <= s_23[0]++;
            System.Console.WriteLine(var5);
        }

        System.Console.WriteLine(arg3);
    }

    static ushort M32()
    {
        return default(ushort);
    }
}
