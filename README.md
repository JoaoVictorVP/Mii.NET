# Mii.NET

## Introduction
Library for using .NET without GC in many cases.

## How to use?
The usage is very intuitive, but here are a very quickly explanation of some types:

### NativeArray
For handling arrays without GC, you can use the NativeArray\<T\> type, you can create one as follows:
```csharp
var narr = new NativeArray<int>(32); // A native array of integers with 32 items (T must be a unmanaged type)
narr[0] = 1000; // Manually assign values to array
narr.Dispose(); // Disposing allocated memory is very important, forgetting to do this would create memory leaks
```
Alternativelly, you can use NativeArray (and other types too) safely with smart usings like:
```csharp
using var narr = new NativeArray<int>(32);
narr[0] = 1000;
```

### NativeList
Lists without using GC, this struct use NativeArray as it backing array and should work with all unmanaged types.
NativeLists works like normal .NET lists, they would double internal backing array capacity every time it's reached and disposing would free this backing array.
To create one:
```csharp
var nlist = new NativeList<byte>(320); // A native list of bytes with capacity of 320 items (capacity is not the same as size, as you should know using .NET lists)
nlist.Add(150);
nlist.Add(255);
nlist.Add(30);
nlist.Dispose(); // Freing allocated memory
```
Of course, you can use smart usings here too:
```csharp
using var nlist = new NativeList<byte>(320); // A native list of bytes with capacity of 320 items (capacity is not the same as size, as you should know using .NET lists)
nlist.Add(150);
nlist.Add(255);
nlist.Add(30);
```
And you can get the current backing array using (however, this is not recommended for most scenarious):
```csharp
...
var narr = nlist.GetArray();
```

### NativeString
Strings without GC in .NET!!! You can use then as regular strings, however they will not need to be GC collected after using (never forget to release them automatically).
You can use as follows:
```csharp
NativeString nstr = "Hello, World!";
...
nstr.Dispose();
```
Using smart pointers:
```csharp
using NativeString nstr = "Hello, World!";
```
Differently from .NET standart strings, you can change internal values of these strings (only change, not increase without creating new versions of string) as follows:
```csharp
using NativeString nstr = "Hello, World!";
nstr[^1] = '?';
Console.WriteLine(nstr); // This should print "Hello, World?"
```
You can, of course, turn them into readonly strings if necessary:
```csharp
using var nstr = ((NativeString)"Hello, World!").AsReadOnly();
nstr[^1] = '?'; // This way, this line will not have any effect on string
Console.WriteLine(nstr); // This should print "Hello, World!"
```

### Ptr
Lastly but not less important, Ptr\<T\>, this is a type that will allow you to unsafelly handle your unmanaged structs without needing to use pointers and 'unsafe' keyword all time. This can be used as follows:
```csharp
struct Position
{
  public int X;
  public int Y;
  
  public override string ToString() => $"Pos(X: {X}, Y: {Y})";
}
...
var npos = new Ptr<Position>(true);
ref var pos = npos.On;
pos.X = 100;
pos.Y = 300;
ref var otherPosRef = npos.On;
Console.WriteLine(otherPosRef.ToString()); // This should print "Pos(X: 100, Y: 300)"
npos.Dispose(); // Do not forget to release it when not using anymore
```
Alternativelly, you can use smart usings too:
```csharp
using var npos = new Ptr<Position>(true);
...
```

### More?
There are other several types for use like SpanStack, SpanStream and SmartPointer that you can use, but I'll not cover them here because they're well documented and should be usable without any further problems.

Any bugs should be reported here, and have a happy codding!
