// Generated by Fuzzlyn on 2018-07-04 09:42:59
// Seed: 4042366659952462539
// Reduced from 15.6 KiB to 0.2 KiB
// Debug: Outputs 65409
// Release: Outputs -127
public class Program
{
    static sbyte s_3;
    public static void Main()
    {
        s_3 = -127;
        short vr12 = 0;
        char vr7 = (char)(vr12 ^ s_3);
        System.Console.WriteLine((int)vr7);
    }
}
