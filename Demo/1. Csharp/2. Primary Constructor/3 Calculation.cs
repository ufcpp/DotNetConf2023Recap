// その他、引数を加工して記録するようなものは record にしづらい。

using System.Numerics;

readonly struct BitField
{
    private readonly byte _bits;

    public BitField(int a, int b, int c)
    {
        _bits = (byte)(
            ((a & 0b11) << 6)
            | ((b & 0b111) << 3)
            | (c & 0b111)
            );
    }

    public int A => _bits >> 6;
    public int B => (_bits >> 3) & 0b111;
    public int C => _bits & 0b111;
}

readonly struct Line
{
    public Complex A { get; }
    public Complex B { get; }

    public Line(double x, double y, double z, double w)
    {
        A = new(x, y);
        B = new(z, w);
    }
}
