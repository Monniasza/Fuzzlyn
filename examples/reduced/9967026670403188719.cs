// Generated by Fuzzlyn v1.2 on 2021-08-06 22:23:33
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 9967026670403188719
// Reduced from 105.1 KiB to 0.3 KiB in 00:02:51
// Debug: Outputs 0
// Release: Outputs 1
public class Program
{
    static int s_1 = 1;
    static long[][] s_4 = new long[][]{new long[]{0}};
    public static void Main()
    {
        M18() &= 0;
        System.Console.WriteLine(s_1);
    }

    static ref int M18()
    {
        s_4[0] = new long[]{0};
        return ref s_1;
    }
}
