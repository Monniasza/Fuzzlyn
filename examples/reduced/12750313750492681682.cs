// Generated by Fuzzlyn v1.2 on 2021-08-05 17:49:18
// Run on Arm64 Linux, .NET 6.0.0-preview.6.21352.12
// Seed: 12750313750492681682
// Reduced from 221.6 KiB to 0.3 KiB in 00:08:09
// Debug: Outputs 0
// Release: Outputs 1
public class Program
{
    public static void Main()
    {
        byte[] vr3 = new byte[]{1};
        for (int vr4 = 0; vr4 < 2; vr4++)
        {
            vr3[0] = 0;
            byte vr5 = vr3[0];
            System.Console.WriteLine(vr5);
        }
    }
}
