// Generated by Fuzzlyn v1.1 on 2018-08-27 14:24:00
// Seed: 18421541889474876981
// Reduced from 175.3 KiB to 0.4 KiB in 00:03:18
// Debug: Outputs 0
// Release: Outputs 1
struct S0
{
    public byte F0;
    public ulong F2;
    public byte F6;
    public S0(byte f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    static S0 s_4;
    static S0 s_24 = new S0(1);
    public static void Main()
    {
        M27(s_4, M27(s_24, s_24));
    }

    static ref S0 M27(S0 arg0, S0 arg1)
    {
        System.Console.WriteLine(arg0.F0);
        return ref s_24;
    }
}
