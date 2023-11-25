public class ScalableTimeProvider(TimeProvider innerProvider) : TimeProvider
{
    public ScalableTimeProvider() : this(System) { }

    private long _startTimestamp = innerProvider.GetTimestamp();
    private long _offsetTimestamp = 0;
    private DateTimeOffset _startUtcNow = innerProvider.GetUtcNow();
    private TimeSpan _offsetUtcNow = default;

    private double _scale = 1;

    public double Scale
    {
        get => _scale;
        set => ChangeScale(value);
    }

    private event Action<double>? ScaleChanged;

    private void ChangeScale(double value)
    {
        if (_scale == value) return;

        var s1 = GetTimestamp();
        var s2 = _startTimestamp = innerProvider.GetTimestamp();
        _offsetTimestamp = s1 - s2;

        var n1 = GetUtcNow();
        var n2 = _startUtcNow = innerProvider.GetUtcNow();
        _offsetUtcNow = n1 - n2;

        _scale = value;
        ScaleChanged?.Invoke(value);
    }

    public override long GetTimestamp()
    {
        var timestamp = innerProvider.GetTimestamp();
        return (long)(_startTimestamp + Scale * (timestamp - _startTimestamp)) + _offsetTimestamp;
    }

    public override DateTimeOffset GetUtcNow()
    {
        var now = innerProvider.GetUtcNow();
        return _startUtcNow + Scale * (now - _startUtcNow) + _offsetUtcNow;
    }

    private class XTimer : ITimer
    {
        private readonly ScalableTimeProvider _parent;
        private readonly ITimer _innerTimer;
        private TimeSpan _period;

        public XTimer(ScalableTimeProvider parent, ITimer innerTimer)
        {
            _parent = parent;
            _innerTimer = innerTimer;

            parent.ScaleChanged += OnScaleChanged;
        }

        private void OnScaleChanged(double scale)
        {
            var inv = 1 / scale;
            _innerTimer.Change(inv * _period, inv * _period);
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            _period = period;
            var inv = 1 / _parent.Scale;
            return _innerTimer.Change(inv * dueTime, inv * period);
        }

        public void Dispose()
        {
            _parent.ScaleChanged -= OnScaleChanged;
            _innerTimer.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            _parent.ScaleChanged -= OnScaleChanged;
            await _innerTimer.DisposeAsync();
        }
    }

    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        var t = new XTimer(this, innerProvider.CreateTimer(callback, state, default, default));
        t.Change(dueTime, period);
        return t;
    }

    public override TimeZoneInfo LocalTimeZone => innerProvider.LocalTimeZone;

    public override long TimestampFrequency => innerProvider.TimestampFrequency;
}
