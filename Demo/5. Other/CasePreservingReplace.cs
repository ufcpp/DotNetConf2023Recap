using System.Runtime.CompilerServices;

class CasePreservingReplace
{
    private int _x1;
    public int X1 { get => _x1; set =>SetProperty(ref _x1, value); }

    private int _x2;
    public int X2 { get => _x2; set => SetProperty(ref _x2, value); }

    private int _x3;
    public int X3 { get => _x3; set => SetProperty(ref _x3, value); }

    private int _x4;
    public int X4 { get => _x4; set => SetProperty(ref _x4, value); }

    private int _x5;
    public int X5 { get => _x5; set => SetProperty(ref _x5, value); }

    private int _x6;
    public int X6 { get => _x6; set => SetProperty(ref _x6, value); }


    public void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        storage = value;
        // PropertyChanged?.Invoke() 呼んだり
    }
}
