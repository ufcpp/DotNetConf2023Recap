using System.Collections.Immutable;

class FixProblem
{
    public void M()
    {
        // これだとまともに動かない。
        ImmutableArray<int> immutable = new() { 1, 2, 3, 4 };
    }
}
