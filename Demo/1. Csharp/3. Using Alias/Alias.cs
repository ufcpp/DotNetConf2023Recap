using T = System.Int32;
using Pair = System.ValueTuple<int, int>;
using List = System.Collections.Generic.List<System.ValueTuple<int, int>>;

class Alias
{
    public static void Run()
    {
        T x = 1;
        T y = 2;

        Pair a = (x, x);
        Pair b = (x, y);
        Pair c = (y, y);

        List list = [a, b, c];

        foreach (var p in list) Console.WriteLine(p);
    }
}
