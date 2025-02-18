public interface IDictionaryPool<TKey, TValue>
{
    int dictionaryCount
    {
        get;
    }
    int countAll
    {
        get;
    }

    int Count(TKey key);
    void Clear(TKey key);
    void Clear();
    TValue Get(TKey key);
    void Release(TKey key, TValue item);
}