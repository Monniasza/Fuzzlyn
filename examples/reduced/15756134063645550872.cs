// Generated by Fuzzlyn on 2018-07-13 08:31:59
// Seed: 15756134063645550872
// Reduced from 32.4 KiB to 0.2 KiB
// Debug: Outputs 65446
// Release: Outputs 4294967206
public class Program
{
    static sbyte[, ] s_1 = new sbyte[, ]{{0}};
    public static void Main()
    {
        s_1[0, 0] = -90;
        ulong vr26 = (char)(0 ^ s_1[0, 0]);
        System.Console.WriteLine(vr26);
    }
}
