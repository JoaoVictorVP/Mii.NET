using IzaBlockchain.Net;

Console.WriteLine("Testing Mii.NET");

var list = new NativeList<int>() { 100, 300, 500 };

Console.WriteLine("Original:\n" + list);

list.Insert(3, 900);

Console.WriteLine("Modified:\n" + list);

Console.ReadKey(true);

unsafe void testForEach()
{
    static void iterate(int index, int item) => Console.WriteLine($"{item} at {index}");

    list.ForEach(&iterate);
}
testForEach();