// Generated by Fuzzlyn v1.1 on 2018-11-16 12:24:47
// Seed: 5020196959077268037
// Reduced from 318.9 KiB to 0.8 KiB in 00:07:29
// Debug: Prints 1 line(s)
// Release: Prints 0 line(s)
struct S1
{
    public uint F0;
    public long F6;
    public int F8;
    public S1(long f6): this()
    {
        F6 = f6;
    }
}

public class Program
{
    static S1[][][] s_39 = new S1[][][]{new S1[][]{new S1[]{new S1(0)}}};
    static ushort s_46 = 1;
    static ushort[] s_90 = new ushort[]{0};
    public static void Main()
    {
        S1 vr13 = s_39[0][0][0];
        var vr9 = new S1(1);
        M85(vr13, M102(vr9));
    }

    static void M85(S1 arg0, S1 arg1)
    {
        if (arg0.F6 < s_46)
        {
            var vr14 = new ushort[]{0};
            System.Console.WriteLine(vr14[0]);
        }
    }

    static ref S1 M102(S1 arg0)
    {
        ushort var1 = s_90[0];
        return ref s_39[0][0][0];
    }
}
