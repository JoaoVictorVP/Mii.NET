using IzaBlockchain.Net;
using System.Diagnostics;

Console.WriteLine("Testing Mii.NET");

var list = new NativeList<int>();

const int iterations = 10_000_000;

Stopwatch sw = new Stopwatch();
sw.Start();
for (int i = 0; i < iterations; i++)
    list.Add(i);
sw.Stop();

Console.WriteLine("Elapsed Time: " + sw.Elapsed);

var nlist = new List<int>();

sw = new Stopwatch();

sw.Start();
for (int i = 0; i < iterations; i++)
    nlist.Add(i);
sw.Stop();

Console.WriteLine("Elapsed Time for normal lists: " + sw.Elapsed);