// Generated by Fuzzlyn v1.1 on 2018-12-29 05:23:46
// Seed: 7463596657932504894
// Reduced from 324.4 KiB to 0.5 KiB in 00:04:51
// Debug: Outputs 1
// Release: Outputs 0
struct S1
{
    public int F0;
    public short F4;
    public short F5;
    public bool F6;
    public long F8;
    public S1(int f0): this()
    {
        F0 = f0;
    }
}

public class Program
{
    static S1 s_7;
    public static void Main()
    {
        var vr8 = new S1(1);
        var vr9 = new S1(0);
        M8(vr8, M8(vr9, s_7));
    }

    static ref S1 M8(S1 arg0, S1 arg1)
    {
        System.Console.WriteLine(arg0.F0);
        return ref s_7;
    }
}
