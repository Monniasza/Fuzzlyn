// Generated by Fuzzlyn v1.2 on 2021-08-15 23:26:42
// Run on .NET 6.0.0-dev on Arm Linux
// Seed: 171224399341698993
// Reduced from 96.8 KiB to 0.4 KiB in 00:03:38
// Debug: Outputs 0
// Release: Outputs -432734208
struct S0
{
    public int F0;
}

class C0
{
}

struct S1
{
    public S0 F0;
    public C0 F1;
    public ushort F2;
}

public class Program
{
    public static void Main()
    {
        S1 vr6 = new S1();
        vr6.F2 = vr6.F2--;
        S1[, ] vr7 = new S1[, ]{{new S1()}};
        vr7[0, 0] = vr6;
        System.Console.WriteLine(vr7[0, 0].F0.F0);
    }
}
