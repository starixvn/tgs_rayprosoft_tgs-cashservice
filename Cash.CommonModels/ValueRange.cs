namespace Tgs.Cash.Common;

public readonly record struct ValueRange<T>(T From, T To) where T : IComparable<T>
{
    public static implicit operator ValueRange<T>((T From, T To) value)
        => new()
        {
            From = value.From,
            To = value.To
        };

    public void Deconstruct(out T from, out T to)
    {
        from = From;
        to = To;
    }
}
