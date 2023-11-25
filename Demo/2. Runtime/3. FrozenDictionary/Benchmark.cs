using System.Collections.Frozen;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Frozen;

// System.Collections.Frozen 以下に、
// FrozenSet と FronzenDictionary が追加された。
//
// 構築フェーズと読み取りフェーズが明確に分かれてるデータ向け。
// 構築に多少時間がかかるものの、読み取り(検索)は高速。
public class Benchmark
{
    // とりあえず比較。
    // 結果: Result.md
    public static void Run()
    {
        BenchmarkRunner.Run<Benchmark>();
    }

    // 普通の。
    //
    // 後からのキー追加の可能性があるなら普通のを使わざるを得ない。
    //
    // 構築: 普通
    // 検索: 普通
    private Dictionary<int, int> _normal = default!;

    // Frozen。
    //
    // 一度構築すると以後追加が無理な代わりに、検索は速い。
    //
    // 構築: ちょっと遅い
    // 検索: 最速
    private FrozenDictionary<int, int> _frozen = default!;

    // Immutable。
    //
    // 要素の追加時に現インスタンスを書き換えず、新しいインスタンスを作る。
    // 並列処理関連でこういう方式が有利なことがある。
    // 普通に使うと遅い。Frozen な用途で勘違いして使われがち。
    // 並列処理があっても大体の用途では ConcurrentDictionary の方が速い。
    // 見かけたらまず誤用を疑った方がいいレベル。
    //
    // 構築: 遅い
    // 検索: たいがい遅い
    private ImmutableDictionary<int, int> _immutable = default!;

    [GlobalSetup]
    public void Init()
    {
        var kvps = Enumerable.Range(0, 50)
            .Select(i => 2 * i)
            .Select(i => KeyValuePair.Create(i, i))
            .ToArray();
        _normal = new Dictionary<int, int>(kvps);
        _frozen = kvps.ToFrozenDictionary();
        _immutable = kvps.ToImmutableDictionary();
    }

    [Benchmark]
    public void Normal()
    {
        for (int i = 0; i < 100; i++)
        {
            _normal.ContainsKey(i);
        }
    }

    [Benchmark]
    public void Frozen()
    {
        for (int i = 0; i < 100; i++)
        {
            _frozen.ContainsKey(i);
        }
    }

    [Benchmark]
    public void Immutable()
    {
        for (int i = 0; i < 100; i++)
        {
            _immutable.ContainsKey(i);
        }
    }
}
