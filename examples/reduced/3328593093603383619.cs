// Generated by Fuzzlyn v1.1 on 2018-08-31 11:46:04
// Seed: 3328593093603383619
// Reduced from 255.0 KiB to 0.4 KiB
// Debug: Outputs 3958
// Release: Outputs 295636854
struct S1
{
    public byte F1;
    public sbyte F4;
    public uint F6;
    public uint F7;
    public long F8;
    public S1(uint f6): this()
    {
        F6 = f6;
    }
}

public class Program
{
    public static void Main()
    {
        S1 vr5 = new S1(295636854U);
        vr5.F8 = (ushort)vr5.F6;
        System.Console.WriteLine(vr5.F8);
    }
}
