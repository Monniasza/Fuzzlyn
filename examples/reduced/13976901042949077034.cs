// Generated by Fuzzlyn on 2018-07-04 19:05:05
// Seed: 13976901042949077034
// Reduced from 213.9 KiB to 0.3 KiB
// Debug: Outputs 241
// Release: Outputs 65521
class C0
{
    public sbyte F1;
    public char F3;
}

public class Program
{
    static C0 s_21 = new C0();
    public static void Main()
    {
        s_21.F1 = -15;
        byte vr38 = (byte)(1L | s_21.F1);
        s_21.F3 = (char)vr38;
        System.Console.WriteLine((int)s_21.F3);
    }
}
