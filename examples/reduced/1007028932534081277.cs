// Generated by Fuzzlyn v1.1 on 2018-09-23 01:04:37
// Seed: 1007028932534081277
// Reduced from 155.3 KiB to 0.5 KiB in 00:04:46
// Debug: Outputs 0
// Release: Outputs 232
public class Program
{
    static sbyte s_8;
    static sbyte[] s_36 = new sbyte[]{-1};
    public static void Main()
    {
        var vr7 = s_36[0];
        uint vr8 = M60(vr7);
        System.Console.WriteLine(vr8);
    }

    static uint M60(sbyte arg0)
    {
        try
        {
            M61();
        }
        finally
        {
            sbyte var6 = arg0--;
        }

        arg0 = s_8;
        return (uint)(arg0 % -116);
    }

    static ref sbyte M61()
    {
        return ref s_8;
    }
}
