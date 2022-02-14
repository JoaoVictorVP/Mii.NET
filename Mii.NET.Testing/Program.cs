using IzaBlockchain.Net;
using System.Diagnostics;
using System.Text;

const string baseMessage = "Hello, World! ";

Console.WriteLine("Testing Mii.NET");

const int iterations = 100000;

//var list = new NativeList<NativeString>();
NativeList<char> strBuilder = new NativeList<char>();

NativeString baseString = baseMessage;
Stopwatch sw = new Stopwatch();
sw.Start();
for (int i = 0; i < iterations; i++)
    strBuilder.Add('A');
baseString = baseString + new NativeString(strBuilder);
sw.Stop();

Console.WriteLine("Elapsed Time: " + sw.Elapsed);

//var nlist = new List<int>(iterations);
//var nlist = new List<string>();

sw = new Stopwatch();

string nBaseString = baseMessage;
//var nStrBuilder = new List<char>();
StringBuilder nStrBuilder = new StringBuilder();
sw.Start();
for (int i = 0; i < iterations * 3; i++)
    nStrBuilder.Append('A');
//nlist.Add(nBaseString = nBaseString.ToUpper());
//nlist.Add(i);
nBaseString = nBaseString + nStrBuilder;
sw.Stop();

Console.WriteLine("Elapsed Time for normal lists: " + sw.Elapsed);