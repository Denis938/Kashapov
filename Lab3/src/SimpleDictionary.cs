using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab3;


public class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
    private class Entry
    {
        public TKey Key;
        public TValue Value;
        public Entry? Next;

        public Entry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
    }

    private Entry?[] _buckets;
    private int _count;
    private const int DefaultCapacity = 16;
    private const float LoadFactor = 0.75f;

    public SimpleDictionary()
    {
        _buckets = new Entry[DefaultCapacity];
        _count = 0;
    }

    public SimpleDictionary(int capacity)
    {
        if (capacity < 1)
            capacity = DefaultCapacity;
        _buckets = new Entry[capacity];
        _count = 0;
    }

    #region IDictionary<TKey, TValue> Implementation

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue? value))
                return value;
            throw new KeyNotFoundException($"Ключ '{key}' не найден");
        }
        set
        {
            int bucketIndex = GetBucketIndex(key);
            Entry? current = _buckets[bucketIndex];

            while (current != null)
            {
                if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
                {
                    current.Value = value;
                    return;
                }
                current = current.Next;
            }

            Add(key, value);
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new SimpleList<TKey>();
            foreach (var kvp in this)
            {
                keys.Add(kvp.Key);
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new SimpleList<TValue>();
            foreach (var kvp in this)
            {
                values.Add(kvp.Value);
            }
            return values;
        }
    }

    public int Count => _count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (ContainsKey(key))
            throw new ArgumentException($"Ключ '{key}' уже существует");

        if (_count >= _buckets.Length * LoadFactor)
        {
            Resize();
        }

        int bucketIndex = GetBucketIndex(key);
        Entry newEntry = new Entry(key, value);
        newEntry.Next = _buckets[bucketIndex];
        _buckets[bucketIndex] = newEntry;
        _count++;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        for (int i = 0; i < _buckets.Length; i++)
        {
            _buckets[i] = null;
        }
        _count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue? value))
        {
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < _count)
            throw new ArgumentException("Недостаточно места");

        int i = arrayIndex;
        foreach (var kvp in this)
        {
            array[i++] = kvp;
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int bucketIndex = GetBucketIndex(key);
        Entry? current = _buckets[bucketIndex];
        Entry? previous = null;

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
            {
                if (previous == null)
                {
                    _buckets[bucketIndex] = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }
                _count--;
                return true;
            }
            previous = current;
            current = current.Next;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item))
        {
            return Remove(item.Key);
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int bucketIndex = GetBucketIndex(key);
        Entry? current = _buckets[bucketIndex];

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
            {
                value = current.Value;
                return true;
            }
            current = current.Next;
        }

        value = default!;
        return false;
    }

    #endregion

    #region IReadOnlyDictionary Implementation

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    #endregion

    #region IEnumerable Implementation

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (Entry? bucket in _buckets)
        {
            Entry? current = bucket;
            while (current != null)
            {
                yield return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                current = current.Next;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region Private Methods

    private int GetBucketIndex(TKey key)
    {
        int hashCode = key.GetHashCode();
        return Math.Abs(hashCode) % _buckets.Length;
    }

    private void Resize()
    {
        Entry?[] oldBuckets = _buckets;
        _buckets = new Entry[oldBuckets.Length * 2];
        _count = 0;

        foreach (Entry? bucket in oldBuckets)
        {
            Entry? current = bucket;
            while (current != null)
            {
                Add(current.Key, current.Value);
                current = current.Next;
            }
        }
    }

    #endregion
}