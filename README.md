# Fuzzlyn
Fuzzlyn is a fuzzer which utilizes Roslyn to generate random C# programs.
It runs these programs on .NET core and ensures that they give the same results when compiled in debug and release mode.

We developed Fuzzlyn as a project for the 2018 Language-Based Security course at Aarhus University.
Using Fuzzlyn, we have found and reported several bugs in RyuJIT used both by .NET Core and the full .NET framework.
We have also found and reported bugs in Roslyn itself.

## Bugs reported

We have reported the following bugs:

* ~~[NullReferenceException thrown for multi-dimensional arrays in release](https://github.com/dotnet/coreclr/issues/18232)~~
* ~~[Wrong integer promotion in release](https://github.com/dotnet/coreclr/issues/18235)~~
* ~~[Cast to ushort is dropped in release](https://github.com/dotnet/coreclr/issues/18238)~~
* ~~[Wrong value passed to generic interface method in release](https://github.com/dotnet/coreclr/issues/18259)~~
* [Constant-folding int.MinValue % -1](https://github.com/dotnet/roslyn/issues/27348)
* ~~[Deterministic program outputs indeterministic results on Linux in release](https://github.com/dotnet/coreclr/issues/18522)~~
* ~~[RyuJIT incorrectly reorders expression containing a CSE, resulting in exception thrown in release](https://github.com/dotnet/coreclr/issues/18770)~~
* ~~[RyuJIT incorrectly narrows value on ARM32/x86 in release](https://github.com/dotnet/coreclr/issues/18780)~~
* ~~[Invalid value numbering when morphing casts that changes signedness after global morph](https://github.com/dotnet/coreclr/issues/18850)~~
* ~~[RyuJIT spills 16 bit value but reloads as 32 bits in ARM32/x86 in release](https://github.com/dotnet/coreclr/issues/18867)~~
* ~~[RyuJIT fails to preserve variable allocated to RCX around shift on x64 in release](https://github.com/dotnet/coreclr/issues/18884)~~
* ~~[RyuJIT: Invalid ordering when assigning ref-return](https://github.com/dotnet/coreclr/issues/19243)~~
* ~~[RyuJIT: Argument written to stack too early on Linux](https://github.com/dotnet/coreclr/issues/19256)~~
* ~~[RyuJIT: Morph forgets about side effects when optimizing casted shift](https://github.com/dotnet/coreclr/issues/19272)~~
* ~~[RyuJIT: By-ref assignment with null leads to runtime crash](https://github.com/dotnet/coreclr/issues/19444)~~
* ~~[RyuJIT: Mishandling of subrange assertion for rewritten call parameter](https://github.com/dotnet/coreclr/issues/19558)~~
* ~~[RyuJIT: Incorrect ordering around Interlocked.Exchange and Interlocked.CompareExchange](https://github.com/dotnet/coreclr/issues/19583)~~
* ~~[RyuJIT: Missing zeroing of upper bits for small struct used in Volatile.Read](https://github.com/dotnet/coreclr/issues/19599)~~
* ~~[RyuJIT: Incorrect 4-byte immediate emitted for shift causes access violation](https://github.com/dotnet/coreclr/issues/19601)~~
* ~~[Finally block belonging to unexecuted try runs anyway](https://github.com/dotnet/roslyn/issues/29481)~~
* ~~[RyuJIT: Bad codegen with multiple field assignments](https://github.com/dotnet/runtime/issues/11559)~~
* ~~[Runtime crash during JIT register allocation in .NET 5](https://github.com/dotnet/runtime/issues/36237)~~
* ~~[[JIT] Runtime crash in fgMakeOutgoingStructArgCopy](https://github.com/dotnet/runtime/issues/36468)~~
* ~~[JIT EH write thru crash](https://github.com/dotnet/runtime/issues/54100)~~
* ~~[Invalid hoisting with refs/array elements proven to be constant](https://github.com/dotnet/runtime/issues/54118)~~
* ~~[JIT misses a zero extension in series of casts](https://github.com/dotnet/runtime/issues/55127)~~
* ~~[Invalid assertion prop in finally](https://github.com/dotnet/runtime/issues/55131)~~
* ~~[JIT incorrectly reorders method call and static field load](https://github.com/dotnet/runtime/issues/55140)~~
* ~~[Assertion failed 'src->IsCnsIntOrI()' with contained bitcast in field store](https://github.com/dotnet/runtime/issues/55141)~~
* ~~[JIT forgets to normalize arg on load](https://github.com/dotnet/runtime/issues/55143)~~
* ~~[JIT: flow opts does an incorrect pred list update](https://github.com/dotnet/runtime/issues/56495)~~
* ~~[ARM64 multiplication/anding by zero seems to be discarded](https://github.com/dotnet/runtime/issues/56930)~~
* ~~[ARM64 incorrect result when expression involves modulo by 1](https://github.com/dotnet/runtime/issues/56935)~~
* ~~[JIT performs invalid jump threading](https://github.com/dotnet/runtime/issues/56979)~~
* ~~[ARM32: runtime crash inside JIT](https://github.com/dotnet/runtime/issues/57061)~~
* ~~[ARM32: Incorrect split passing of promoted structs with padding](https://github.com/dotnet/runtime/issues/57064)~~
* ~~[ARM64: Incorrect sub expression reordering of tree containing CSE def](https://github.com/dotnet/runtime/issues/57121)~~
* ~~[JIT: Invalid negated result](https://github.com/dotnet/runtime/issues/57640)~~
* ~~[JIT: Assertion failed '!m_VariableLiveRanges->back().m_EndEmitLocation.Valid()'](https://github.com/dotnet/runtime/issues/57752)~~
* ~~[JIT: Assertion failed 'offset != BAD_STK_OFFS'](https://github.com/dotnet/runtime/issues/57767)~~
* ~~[JIT: Assertion failed 'm_compGenTreeID == m_compiler->compGenTreeID'](https://github.com/dotnet/runtime/issues/57775)~~
* ~~[ARM64: Assertion failed 'interval->isWriteThru'/runtime crash in release](https://github.com/dotnet/runtime/issues/58083)~~
* ~~[ARM32: Runtime crash with interfaces](https://github.com/dotnet/runtime/issues/58293)~~
* ~~[JIT still seems to be hoisting some modified indirs](https://github.com/dotnet/runtime/issues/58877)~~
* ~~[RyuJIT: Invalidly optimized negated division](https://github.com/dotnet/runtime/issues/60297)~~


Fuzzlyn has found many thousands of programs producing deviating behavior. Some of the first examples we found can be seen in the [examples folder in the v1.0 tag](https://github.com/jakobbotsch/Fuzzlyn/tree/v1.0/examples) (most of these have since been fixed). To take a couple of them, Fuzzlyn automatically found and produced the following programs:

```csharp
// Generated by Fuzzlyn on 2018-06-03 16:17:09
// Seed: 10744458083861091494
// Reduced from 21.3 KB to 0.2 KB
// Debug: Outputs '246'
// Release: Outputs '4294967286'
public class Program
{
    static sbyte s_1 = -10;
    public static void Main()
    {
        ulong vr44 = (byte)(0U ^ s_1);
        System.Console.WriteLine(vr44);
    }
}
```

 ```csharp
// Generated by Fuzzlyn on 2018-06-03 16:15:22
// Seed: 10187462581749713401
// Reduced from 186.5 KB to 0.2 KB
// Debug: Runs successfully
// Release: Throws 'System.DivideByZeroException'
public class Program
{
    public static void Main()
    {
        var vr219 = 'N' % ((35815 / M1(new ushort[]{65535})) | 1);
    }

    static ushort M1(ushort[] arg2)
    {
        return arg2[0];
    }
}
 ```

## Supported constructs

Fuzzlyn generates only a limited subset of C#.
In particular, it currently generates only very simple loops with constant upper bounds.
It is not a goal to support all C# constructs in the generator.

## Using Fuzzlyn to find bugs

Invoking Fuzzlyn generally requires passing a host and how long to run for (either in number of programs or seconds).
To run a Fuzzlyn instance that generates a million programs and tries to find bugs in these, using the specified dotnet.exe or corerun.exe host, use the following command line (here `--parallelism -1` means to use as many workers as there are processor cores/threads).

```powershell
dotnet fuzzlyn.dll --host <path to dotnet.exe or corerun.exe under test> --num-programs 1000000 --parallelism -1
```

To specify a time to run for, use the `--seconds-to-run` switch instead of the `--num-programs` switch.

The command will produce various progress messages on stdout and once an example is found will output details about it as well.
Additionally the `--output-events-to` argument can be used to specify a file to output events to.
Each event is written on a single line as a JSON object.
An event is emitted when an example is found and also when the run finishes with some statistics about the run.

## Regenerating full programs

When a seed has been obtained the full program can be regenerated by doing the following:

```powershell
dotnet fuzzlyn.dll --seed <seed here> --output-source
```

This will generate exactly the code that produced deviating behavior at runtime.
However, these examples cannot be run directly because they include checksumming of variables, which requires the `IRuntime` interface to be passed to the `Main` method.
Instead, you can disable checksumming by passing the `--checksum-` (note the minus) switch:

```powershell
dotnet fuzzlyn.dll --seed <seed here> --output-source --checksum-
```

These examples are not very useful because they are very big.
For that reason, Fuzzlyn includes an automatic reducer, which takes a seed and reduces the program specified by that seed to something smaller, while retaining the interesting behavior.

## Reducing programs

To reduce a program, use the `--reduce` switch:

```powershell
dotnet fuzzlyn.dll --host <path to dotnet.exe or corerun.exe under test> --seed <seed here> --reduce
```

Depending on the size of the program and error mode this will take a while.
For silent bad codegen examples, it usually does not take longer than a few minutes.
However, if the example crashes the runtime then reduction can take several magnitudes longer than that since a new process needs to be launched for every execution.

The output of this command will be a small C# program that includes information about its seed, size and runtime behavior.

## Reproducing errors in reduced programs

The reduced programs produced by the `--reduce` switch are not the original reduced programs that caused deviating behavior.
Most specifically, the programs have had their checksum calls replaced by `System.Console.WriteLine` calls, and other checksumming related code removed.
This means that the files can be compiled directly with `csc.exe`, or pasted into VS directly.
However, it also means that in very rare cases, the examples do not actually reproduce the errors they are supposed to.
One such case is the following (note that while this is a worked example, the underlying problem has since been fixed in .NET):

```csharp
// Generated by Fuzzlyn on 2018-06-03 16:15:22
// Seed: 1019504228635510285
// Reduced from 154.8 KB to 0.9 KB
// Debug: Outputs '10402227607262999317'
// Release: Throws 'System.NullReferenceException'
struct S0
{
    public uint F0;
    public char F1;
    public int F2;
    public int F3;
    public uint F4;
    public ulong F5;
    public short F6;
    public byte F7;
    public S0(uint f0, char f1, int f2, int f3, uint f4, ulong f5, short f6, byte f7)
    {
        F0 = f0;
        F1 = f1;
        F2 = f2;
        F3 = f3;
        F4 = f4;
        F5 = f5;
        F6 = f6;
        F7 = f7;
    }
}

public class Program
{
    static int[, ] s_1 = new int[, ]{{-1860696896}};
    public static void Main()
    {
        var vr196 = new S0(0U, 'Y', 0, 876181050, 2606508611U, 10402227607262999317UL, 15399, 254);
        var vr197 = (uint)(0U & s_1[0, 0]);
        var vr198 = s_1[0, 0];
        M20(vr196, vr197);
    }

    static int M20(S0 arg0, uint arg2)
    {
        System.Console.WriteLine(arg0.F5);
        return -2147483647;
    }
}
```

In these cases it is easy to introduce the deviating behavior by reintroducing some of the code and class hierarchies used by the checksumming:

```csharp
// Generated by Fuzzlyn on 2018-06-03 16:15:22
// Seed: 1019504228635510285
// Reduced from 154.8 KB to 0.9 KB
// Debug: Outputs '10402227607262999317'
// Release: Throws 'System.NullReferenceException'
struct S0
{
    public uint F0;
    public char F1;
    public int F2;
    public int F3;
    public uint F4;
    public ulong F5;
    public short F6;
    public byte F7;
    public S0(uint f0, char f1, int f2, int f3, uint f4, ulong f5, short f6, byte f7)
    {
        F0 = f0;
        F1 = f1;
        F2 = f2;
        F3 = f3;
        F4 = f4;
        F5 = f5;
        F6 = f6;
        F7 = f7;
    }
}

interface IRuntime
{
    void WriteLine<T>(T val);
}

class Runtime : IRuntime
{
    public void WriteLine<T>(T val) => System.Console.WriteLine(val);
}

public class Program
{
    static IRuntime s_rt;
    static int[, ] s_1 = new int[, ]{{-1860696896}};
    public static void Main()
    {
        s_rt = new Runtime();
        var vr196 = new S0(0U, 'Y', 0, 876181050, 2606508611U, 10402227607262999317UL, 15399, 254);
        var vr197 = (uint)(0U & s_1[0, 0]);
        var vr198 = s_1[0, 0];
        M20(vr196, vr197);
    }

    static int M20(S0 arg0, uint arg2)
    {
        s_rt.WriteLine(arg0.F5);
        return -2147483647;
    }
}
```

While this is still not exactly the same as if Fuzzlyn generated it, it reproduces the error described at the start of the file.
