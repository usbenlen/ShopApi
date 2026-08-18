namespace Shop.Application.Interfaces.Cache;

public enum CacheLookupStatus { 
    Miss,
    Hit,
    Unavailable 
}

public readonly record struct CacheLookupResult<T>(CacheLookupStatus Status, T? Value)
{
    public bool IsHit => Status == CacheLookupStatus.Hit;
    public bool IsMiss => Status == CacheLookupStatus.Miss;
    public bool IsUnavailable => Status == CacheLookupStatus.Unavailable;

    public static CacheLookupResult<T> Hit(T? value) => new(CacheLookupStatus.Hit, value);
    public static CacheLookupResult<T> Miss() => new(CacheLookupStatus.Miss, default);
    public static CacheLookupResult<T> Unavailable() => new(CacheLookupStatus.Unavailable, default);
}