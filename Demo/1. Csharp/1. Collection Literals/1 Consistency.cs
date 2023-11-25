using System.Collections.Immutable;

class Consistency
{
    public void M()
    {
        int[] Array = new[] { 1, 2, 3, 4 };
        List<int> List = new() { 1, 2, 3, 4 };
        Span<int> span = stackalloc[] { 1, 2, 3, 4 };
        ReadOnlySpan<int> ros = new[] { 1, 2, 3, 4 };
        ImmutableArray<int> immutable = ImmutableArray.Create(new[] { 1, 2, 3, 4 });
    }
}
