// Generated by Fuzzlyn v1.2 on 2021-08-13 15:46:27
// Run on .NET 6.0.0-dev on Arm Linux
// Seed: 2525337196584043719
// Reduced from 235.2 KiB to 0.5 KiB in 00:03:28
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public long F0;
}

struct S1
{
    public sbyte F0;
    public int F1;
    public S0 F5;
    public S1(sbyte f0, S0 f5): this()
    {
        F0 = f0;
        F5 = f5;
    }
}

struct S5
{
    public S1 F0;
    public S5(S1 f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    public static void Main()
    {
        S5 vr0 = new S5(new S1(1, new S0()));
        System.Console.WriteLine(vr0.F0.F0);
        System.Console.WriteLine(vr0.F0.F1);
    }
}
