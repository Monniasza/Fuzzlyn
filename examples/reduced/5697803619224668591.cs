// Generated by Fuzzlyn v1.2 on 2021-08-05 21:22:53
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 5697803619224668591
// Reduced from 64.9 KiB to 0.3 KiB in 00:01:00
// Debug: Outputs 0
// Release: Outputs 1
class C0
{
    public uint F4;
    public C0(uint f4)
    {
        F4 = f4;
    }
}

public class Program
{
    public static void Main()
    {
        C0[, ] vr2 = new C0[, ]{{new C0(1)}};
        vr2[0, 0].F4 &= 0;
        C0 vr3 = vr2[0, 0];
        uint vr5 = vr3.F4;
        System.Console.WriteLine(vr5);
    }
}
