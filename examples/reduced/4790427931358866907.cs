// Generated by Fuzzlyn on 2018-07-04 15:49:07
// Seed: 4790427931358866907
// Reduced from 49.4 KiB to 0.2 KiB
// Debug: Outputs 65458
// Release: Outputs -78
public class Program
{
    static sbyte s_2 = -114;
    public static void Main()
    {
        char vr21 = '<';
        char vr22 = (char)(s_2 ^ vr21);
        System.Console.WriteLine((int)vr22);
    }
}
