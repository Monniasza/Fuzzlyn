// Generated by Fuzzlyn on 2018-06-25 09:18:29
// Seed: 17294436460926314716
// Reduced from 57.8 KiB to 0.5 KiB
// Debug: Outputs 1
// Release: Outputs 0
struct S0
{
    public long F8;
    public S0(long f8): this()
    {
        F8 = f8;
    }
}

struct S1
{
    public S0 F0;
    public S1(S0 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    static S1 s_11 = new S1(new S0(1L));
    static short s_13 = -12495;
    public static void Main()
    {
        bool vr28 = (ushort)(s_13 | 1U) <= 0;
        if (vr28)
        {
            s_11.F0.F8 = 0;
        }

        var vr21 = s_11.F0;
        System.Console.WriteLine(vr21.F8);
    }
}
