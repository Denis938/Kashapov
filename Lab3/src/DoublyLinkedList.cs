using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab3;


public class DoublyLinkedList<T> : IList<T>, IReadOnlyList<T>
{
    private class Node
    {
        public T Value;
        public Node? Previous;
        public Node? Next;

        public Node(T value)
        {
            Value = value;
        }
    }

    private Node? _head;
    private Node? _tail;
    private int _count;

    public DoublyLinkedList()
    {
        _head = null;
        _tail = null;
        _count = 0;
    }

    #region IList<T> Implementation

    public T this[int index]
    {
        get => GetNodeAt(index).Value;
        set => GetNodeAt(index).Value = value;
    }

    public int Count => _count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        Node newNode = new Node(item);

        if (_tail == null)
        {
            _head = _tail = newNode;
        }
        else
        {
            newNode.Previous = _tail;
            _tail.Next = newNode;
            _tail = newNode;
        }
        _count++;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index == _count)
        {
            Add(item);
            return;
        }

        Node newNode = new Node(item);

        if (index == 0)
        {
            newNode.Next = _head;
            if (_head != null)
                _head.Previous = newNode;
            _head = newNode;
            if (_tail == null)
                _tail = newNode;
        }
        else
        {
            Node current = GetNodeAt(index);
            newNode.Previous = current.Previous;
            newNode.Next = current;
            current.Previous!.Next = newNode;
            current.Previous = newNode;
        }
        _count++;
    }

    public bool Remove(T item)
    {
        Node? current = _head;
        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, item))
            {
                RemoveNode(current);
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        Node node = GetNodeAt(index);
        RemoveNode(node);
    }

    public int IndexOf(T item)
    {
        Node? current = _head;
        int index = 0;
        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, item))
                return index;
            current = current.Next;
            index++;
        }
        return -1;
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    public void Clear()
    {
        _head = null;
        _tail = null;
        _count = 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || array.Length - arrayIndex < _count)
            throw new ArgumentException("Invalid array or index");

        Node? current = _head;
        while (current != null)
        {
            array[arrayIndex++] = current.Value;
            current = current.Next;
        }
    }

    #endregion

    #region IEnumerable Implementation

    public IEnumerator<T> GetEnumerator()
    {
        Node? current = _head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Private Methods

    private Node GetNodeAt(int index)
    {
        if (index < 0 || index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        Node current;

        if (index < _count / 2)
        {
            current = _head!;
            for (int i = 0; i < index; i++)
                current = current.Next!;
        }
        else
        {
            current = _tail!;
            for (int i = _count - 1; i > index; i--)
                current = current.Previous!;
        }

        return current;
    }

    private void RemoveNode(Node node)
    {
        if (node.Previous != null)
            node.Previous.Next = node.Next;
        else
            _head = node.Next;

        if (node.Next != null)
            node.Next.Previous = node.Previous;
        else
            _tail = node.Previous;

        _count--;
    }

    #endregion

    #region Additional Methods

    public void AddFirst(T item)
    {
        Insert(0, item);
    }

    public void AddLast(T item)
    {
        Add(item);
    }

    #endregion
}