// Generated by Fuzzlyn v1.2 on 2021-08-07 02:54:19
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 10593415732872235704
// Reduced from 117.1 KiB to 0.3 KiB in 00:02:45
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public int F1;
    public S0(int f1): this()
    {
        F1 = f1;
    }
}

public class Program
{
    public static void Main()
    {
        var vr1 = new S0(1);
        vr1.F1 &= 0;
        System.Console.WriteLine(vr1.F1);
    }
}
