// Generated by Fuzzlyn v1.1 on 2018-09-29 22:49:51
// Seed: 18222898030821758256
// Reduced from 302.6 KiB to 0.6 KiB in 00:10:36
// Debug: Outputs 0
// Release: Outputs 2
class C0
{
    public sbyte F2;
}

public class Program
{
    static byte s_73;
    static C0 s_102 = new C0();
    public static void Main()
    {
        sbyte vr5 = s_102.F2;
        M107(vr5);
    }

    static void M107(sbyte arg0)
    {
        try
        {
            M108();
        }
        finally
        {
            arg0 = arg0++;
            arg0 = (sbyte)(-1 - s_73);
            arg0 /= 123;
        }

        System.Console.WriteLine(arg0);
    }

    static int M108()
    {
        return default(int);
    }
}
