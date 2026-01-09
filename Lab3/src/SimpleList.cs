using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab3;


public class SimpleList<T> : IList<T>, ICollection<T>, IEnumerable<T>
{
    private T[] _items;
    private int _count;
    private const int DefaultCapacity = 4;

    public SimpleList()
    {
        _items = new T[DefaultCapacity];
        _count = 0;
    }

    public SimpleList(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));
        _items = new T[capacity];
        _count = 0;
    }

    #region IList<T> Implementation

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            _items[index] = value;
        }
    }

    public int IndexOf(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_items[i], item))
                return i;
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        EnsureCapacity(_count + 1);

        for (int i = _count; i > index; i--)
        {
            _items[i] = _items[i - 1];
        }

        _items[index] = item;
        _count++;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        for (int i = index; i < _count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        _items[_count - 1] = default!;
        _count--;
    }

    #endregion

    #region ICollection<T> Implementation

    public int Count => _count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        EnsureCapacity(_count + 1);
        _items[_count] = item;
        _count++;
    }

    public void Clear()
    {
        for (int i = 0; i < _count; i++)
        {
            _items[i] = default!;
        }
        _count = 0;
    }

    public bool Contains(T item)
    {
        return IndexOf(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < _count)
            throw new ArgumentException("Недостаточно места в целевом массиве");

        for (int i = 0; i < _count; i++)
        {
            array[arrayIndex + i] = _items[i];
        }
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    #endregion

    #region IEnumerable<T> Implementation

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region Private Methods

    private void EnsureCapacity(int minCapacity)
    {
        if (_items.Length < minCapacity)
        {
            int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
            if (newCapacity < minCapacity)
                newCapacity = minCapacity;

            T[] newItems = new T[newCapacity];
            for (int i = 0; i < _count; i++)
            {
                newItems[i] = _items[i];
            }
            _items = newItems;
        }
    }

    #endregion

    public int Capacity => _items.Length;
}