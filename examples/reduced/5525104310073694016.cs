// Generated by Fuzzlyn v1.2 on 2021-08-06 21:09:41
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 5525104310073694016
// Reduced from 34.5 KiB to 0.3 KiB in 00:01:03
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public int F0;
    public S0(int f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    static S0 s_13 = new S0(1);
    public static void Main()
    {
        S0 vr0 = new S0(0);
        s_13.F0 *= vr0.F0;
        System.Console.WriteLine(s_13.F0);
    }
}
