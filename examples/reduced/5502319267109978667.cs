// Generated by Fuzzlyn v1.1 on 2018-09-03 00:14:52
// Seed: 5502319267109978667
// Reduced from 114.2 KiB to 0.7 KiB in 00:04:21
// Debug: Outputs 0
// Release: Outputs 25
struct S0
{
    public int F0;
    public byte F1;
    public ushort F7;
}

public class Program
{
    static S0 s_15;
    public static void Main()
    {
        var vr3 = (sbyte)s_15.F0;
        var vr4 = new byte[][]{new byte[]{0}};
        M26(vr3, vr4);
        System.Console.WriteLine(s_15.F7);
    }

    static void M26(sbyte arg0, byte[][] arg1)
    {
        try
        {
            arg1[0][0] = arg1[0][0];
        }
        finally
        {
            arg1[0][0] = arg1[0][0];
            bool var0 = true;
            arg1[0][0] = s_15.F1;
            arg1 = new byte[][]{new byte[]{1}};
            System.Console.WriteLine(var0);
        }

        --arg0;
        s_15.F7 = (ushort)(arg0 / 10);
    }
}
