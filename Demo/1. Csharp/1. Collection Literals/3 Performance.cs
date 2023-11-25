class Performance
{
    public void M()
    {
        // これだと配列の new が重たい。
        Span<int> span = new[] { 1, 2, 3, 4 };

        // これで「最適化で配列の new が消える」と言われても信じきれない。
        ReadOnlySpan<int> ros = new[] { 1, 2, 3, 4 };
    }
}
