// Generated by Fuzzlyn v1.1 on 2018-09-21 08:49:31
// Seed: 14635215519539154909
// Reduced from 240.2 KiB to 0.4 KiB in 00:04:46
// Debug: Outputs 0
// Release: Outputs 2
public class Program
{
    static uint[] s_2 = new uint[]{0};
    public static void Main()
    {
        M23(ref s_2[0], 0);
    }

    static void M23(ref uint arg3, short arg5)
    {
        try
        {
            s_2[0] = arg3;
        }
        finally
        {
            var vr2 = arg5--;
            var vr3 = arg5 / 32767;
            System.Console.WriteLine(vr3);
        }
    }
}
