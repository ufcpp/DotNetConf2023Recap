class MyOwnTimeProvider
{
    public static async Task RunAsync()
    {
        var cts = new CancellationTokenSource();

        // 引数で渡す TimeProvider を変更。

        var scaled = new ScalableTimeProvider() { Scale = 1 };
        var pausable = new PausableTimeProvider(scaled);

        var task = new Loop(pausable).RunAsync(cts.Token);

        Console.Write("""
            p: pause
            r: resume
            1-9: change scale
            q: quit

            """);

        while (true)
        {
            var c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (c is 'p')
            {
                Console.WriteLine("paused");
                pausable.IsPaused = true;
            }
            else if (c is 'r')
            {
                Console.WriteLine("resumed");
                pausable.IsPaused = false;
            }
            else if (c is >= '1' and <= '9')
            {
                var scale = c - '0';
                Console.WriteLine($"scale changed to {scale}");
                scaled.Scale = scale;
            }
            else if (c is 'q')
            {
                break;
            }
        }

        cts.Cancel();
        await task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }
}
