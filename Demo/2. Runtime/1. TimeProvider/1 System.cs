class SystemTimeProvider
{
    public static async Task RunAsync()
    {
        var cts = new CancellationTokenSource();

        var tp = TimeProvider.System;

        var task = new Loop(tp).RunAsync(cts.Token);

        Console.Write("""
            q: quit

            """);

        while (true)
        {
            var c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (c is 'q')
            {
                break;
            }
        }

        cts.Cancel();
        await task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }
}
