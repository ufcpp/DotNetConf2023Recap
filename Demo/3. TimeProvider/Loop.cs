/// <summary>
/// 0.5 秒に1回時刻を表示するループ。
/// </summary>
class Loop(TimeProvider timeProvider)
{
    public async Task RunAsync(CancellationToken ct)
    {
        var t = new PeriodicTimer(TimeSpan.FromMilliseconds(500), timeProvider);

        while (!ct.IsCancellationRequested)
        {
            await t.WaitForNextTickAsync(ct);
            Console.WriteLine(timeProvider.GetLocalNow());
        }
    }
}
