// Generated by Fuzzlyn v1.2 on 2021-08-12 10:21:43
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 7952150441604559931
// Reduced from 11.3 KiB to 0.4 KiB in 00:00:32
// Debug: Outputs 1
// Release: Outputs 0
public class Program
{
    static ushort[][, ] s_1 = new ushort[][, ]{new ushort[, ]{{1}}};
    public static void Main()
    {
        M6(1);
        System.Console.WriteLine(s_1[0][0, 0]);
    }

    static void M6(ushort arg0)
    {
        if (1 > (1 + (1044619672059493453UL % arg0--)))
        {
            s_1[0][0, 0] = 0;
        }
    }
}
