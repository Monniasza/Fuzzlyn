// Generated by Fuzzlyn v1.2 on 2021-08-09 15:54:25
// Run on .NET 6.0.0-dev on Arm64 Linux
// Seed: 400187736390402870
// Reduced from 38.4 KiB to 0.7 KiB in 00:01:02
// Debug: Outputs 1
// Release: Outputs 0
public class Program
{
    static ushort s_1;
    static bool[] s_2 = new bool[]{true};
    public static void Main()
    {
        if (s_2[0])
        {
            ref ushort vr3 = ref s_1;
            ushort vr4 = vr3--;
            short vr5 = 0;
            if ((1 + ((int)(vr5 - (uint)M1(ref s_1)) % 1)) > M1(ref s_1))
            {
                long vr1 = (-441112005 % M2()) + 1;
                System.Console.WriteLine(vr1);
            }
        }
    }

    static short M1(ref ushort arg0)
    {
        ushort var0 = arg0--;
        return 0;
    }

    static sbyte M2()
    {
        var vr0 = new byte[]{1, 1, 0, 0};
        return 1;
    }
}
