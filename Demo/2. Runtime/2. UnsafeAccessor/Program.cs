using System.Runtime.CompilerServices;

namespace UnsafeAccessorExample;

// init-only な型。
readonly record struct Point(int X, int Y);

// デシリアライズとかで、private とか readonly なフィールドを無理やり書き換えたいことがまれに。
//
// これまでだとリフレクションで書き換えたりしてた。
// 最近、AOT シナリオでリフレクションが使いづらく、静的な「裏口」が求められてた。
class PointProxy
{
    // UnsafeAccessor がその裏口。
    // JIT/AOT 時解決のリフレクション代替手段。
    //
    // この例は「自動プロパティ X に対して <X>k__BackingField というフィールドができてる」という
    // undocumented な挙動に依存したコード。
    // いつ変更されても文句は言えないので、限られたシナリオでしか使えない。
    //
    // 想定されているのは「AOT 前提で Source Generator とかから使う」。
    // 他に、「単体テストで internal メンバーのテストをしたい」とかに使えそう。
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<X>k__BackingField")]
    public extern static ref int X(ref Point p);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Y>k__BackingField")]
    public extern static ref int Y(ref Point p);
}

class Program
{
    // 利用例。
    public static void Run()
    {
        // デシリアライザーだと、最初に default でインスタンスを作って、
        Point p = default;

        // 後からメンバーの set をしたいみたいな実装があったりする。
        // init-only プロパティとかだと「裏口」が必要。
        PointProxy.X(ref p) = 1;
        PointProxy.Y(ref p) = 2;

        Console.WriteLine(p);
    }
}
