// Generated by Fuzzlyn v1.2 on 2021-08-06 18:49:35
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 7043115980116891592
// Reduced from 16.4 KiB to 0.3 KiB in 00:00:37
// Debug: Outputs 0
// Release: Outputs 1
class C0
{
    public int F1;
    public C0(int f1)
    {
        F1 = f1;
    }
}

public class Program
{
    public static void Main()
    {
        var vr1 = new C0(1);
        M1(vr1);
    }

    static void M1(C0 arg0)
    {
        arg0.F1 &= 0;
        System.Console.WriteLine(arg0.F1);
    }
}
