// Generated by Fuzzlyn v1.1 on 2018-08-02 16:13:21
// Seed: 4370926422548428545
// Reduced from 0.9 KiB to 0.2 KiB
// Debug: Runs successfully
// Release: Throws 'System.NullReferenceException'
public class Program
{
    static ushort[, ] s_2 = new ushort[, ]{{0}};
    public static void Main()
    {
        ulong vr0 = 0 / (549989829U | (uint)(0 & s_2[0, 0]));
        long vr1 = s_2[0, 0];
    }
}
