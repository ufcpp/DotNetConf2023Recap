/// <summary>
/// 一時停止ができる <see cref="TimeProvider"/>
/// </summary>
public class PausableTimeProvider(TimeProvider innerProvider) : TimeProvider
{
    public PausableTimeProvider() : this(System) { }

    private long _pausedTimestamp = innerProvider.GetTimestamp();
    private long _offsetTimestamp = 0;
    private DateTimeOffset _pausedUtcNow = innerProvider.GetUtcNow();
    private TimeSpan _offsetUtcNow = default;

    private bool _isPaused = false;

    public bool IsPaused
    {
        get => _isPaused;
        set => ChangePaused(value);
    }

    private event Action<bool>? PauseChanged;

    private void ChangePaused(bool value)
    {
        if (_isPaused == value) return;

        if (value)
        {
            _pausedTimestamp = innerProvider.GetTimestamp();
            _pausedUtcNow = innerProvider.GetUtcNow();
        }
        else
        {
            var timestamp = innerProvider.GetTimestamp();
            _offsetTimestamp += timestamp - _pausedTimestamp;

            var now = innerProvider.GetUtcNow();
            _offsetUtcNow += now - _pausedUtcNow;
        }

        _isPaused = value;
        PauseChanged?.Invoke(value);
    }

    public override long GetTimestamp()
    {
        if (IsPaused) return _pausedTimestamp;

        var timestamp = innerProvider.GetTimestamp();
        return timestamp - _offsetTimestamp;
    }

    public override DateTimeOffset GetUtcNow()
    {
        var now = innerProvider.GetUtcNow();
        return now - _offsetUtcNow;
    }

    private class XTimer : ITimer
    {
        private readonly PausableTimeProvider _parent;
        private readonly ITimer _innerTimer;
        private TimeSpan _period;

        public XTimer(PausableTimeProvider parent, ITimer innerTimer, TimeSpan period)
        {
            _parent = parent;
            _innerTimer = innerTimer;
            _period = period;

            parent.PauseChanged += OnPauseChanged;
        }

        private void OnPauseChanged(bool isPaused)
        {
            if (isPaused)
            {
                _innerTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }
            else
            {
                _innerTimer.Change(default, _period); // now - _pausedUtcNow 受け取って due の調整する方がいいかも。
            }
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            if (_parent.IsPaused)
            {
                _period = period;
                return true;
            }
            else
            {
                return _innerTimer.Change(dueTime, period);
            }
        }

        public void Dispose()
        {
            _parent.PauseChanged -= OnPauseChanged;
            _innerTimer.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            _parent.PauseChanged -= OnPauseChanged;
            await _innerTimer.DisposeAsync();
        }
    }

    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        return new XTimer(this, innerProvider.CreateTimer(callback, state, dueTime, period), period);
    }

    public override TimeZoneInfo LocalTimeZone => innerProvider.LocalTimeZone;

    public override long TimestampFrequency => innerProvider.TimestampFrequency;
}
