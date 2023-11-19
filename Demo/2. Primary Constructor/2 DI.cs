// DI ではまず public プロパティにしないので record にはしない。

using Microsoft.Extensions.Logging;

public class SampleController
{
    private readonly ILogger<SampleController> _logger;
    private readonly TimeProvider _timeProvider;

    public SampleController(ILogger<SampleController> logger, TimeProvider timeProvider)
    {
        _logger = logger;
        _timeProvider = timeProvider;
    }

    private int _count;

    public string Get()
    {
        var count = ++_count;
        var now = _timeProvider.GetUtcNow();
        _logger.LogTrace("Get {count} {now}", count, now);
        return $"{count} {now}";
    }
}
