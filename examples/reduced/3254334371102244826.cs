// Generated by Fuzzlyn on 2018-07-26 08:05:10
// Seed: 3254334371102244826
// Reduced from 23.3 KiB to 0.4 KiB
// Debug: Runs successfully
// Release: Throws 'System.NullReferenceException'
class C0
{
    public ushort F1;
}

struct S2
{
    public int F2;
    public C0 F3;
    public S2(int f2, C0 f3): this()
    {
        F2 = f2;
        F3 = f3;
    }
}

public class Program
{
    static S2[, ] s_2 = new S2[, ]{{new S2(378946490, new C0())}};
    public static void Main()
    {
        var vr12 = (0 & s_2[0, 0].F3.F1) / s_2[0, 0].F2;
    }
}
