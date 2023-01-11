namespace Tgs.CashService;

public readonly struct ValueRange<T> where T : IComparable<T>
{
    public T From { get; init; }

    public T To { get; init; }

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
