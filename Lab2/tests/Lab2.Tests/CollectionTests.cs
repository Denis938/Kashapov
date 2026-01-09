using System.Collections.Immutable;
using Xunit;

namespace Lab2.Tests;

public class ListTests
{
    [Fact]
    public void Add_AddsElementToEnd()
    {
        var list = new List<int> { 1, 2, 3 };
        
        list.Add(4);
        
        Assert.Equal(4, list.Count);
        Assert.Equal(4, list[3]);
    }

    [Fact]
    public void Insert_AtBeginning_ShiftsElements()
    {
        var list = new List<int> { 1, 2, 3 };
        
        list.Insert(0, 0);
        
        Assert.Equal(4, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(1, list[1]);
    }

    [Fact]
    public void Insert_AtMiddle_ShiftsElements()
    {
        var list = new List<int> { 1, 2, 3, 4 };
        
        list.Insert(2, 100);
        
        Assert.Equal(5, list.Count);
        Assert.Equal(100, list[2]);
        Assert.Equal(3, list[3]);
    }

    [Fact]
    public void RemoveAt_End_RemovesLastElement()
    {
        var list = new List<int> { 1, 2, 3 };
        
        list.RemoveAt(list.Count - 1);
        
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[1]);
    }

    [Fact]
    public void RemoveAt_Beginning_ShiftsElements()
    {
        var list = new List<int> { 1, 2, 3 };
        
        list.RemoveAt(0);
        
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
    }

    [Fact]
    public void Contains_ExistingElement_ReturnsTrue()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        
        Assert.True(list.Contains(3));
    }

    [Fact]
    public void Contains_NonExistingElement_ReturnsFalse()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        
        Assert.False(list.Contains(100));
    }

    [Fact]
    public void IndexAccess_ReturnsCorrectElement()
    {
        var list = new List<int> { 10, 20, 30, 40, 50 };
        
        Assert.Equal(10, list[0]);
        Assert.Equal(30, list[2]);
        Assert.Equal(50, list[4]);
    }
}

public class LinkedListTests
{
    [Fact]
    public void AddLast_AddsElementToEnd()
    {
        var list = new LinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(3, list.Last.Value);
    }

    [Fact]
    public void AddFirst_AddsElementToBeginning()
    {
        var list = new LinkedList<int>();
        list.AddFirst(1);
        list.AddFirst(2);
        list.AddFirst(3);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(3, list.First.Value);
        Assert.Equal(1, list.Last.Value);
    }

    [Fact]
    public void RemoveLast_RemovesLastElement()
    {
        var list = new LinkedList<int>(new[] { 1, 2, 3 });
        
        list.RemoveLast();
        
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list.Last.Value);
    }

    [Fact]
    public void RemoveFirst_RemovesFirstElement()
    {
        var list = new LinkedList<int>(new[] { 1, 2, 3 });
        
        list.RemoveFirst();
        
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list.First.Value);
    }

    [Fact]
    public void Contains_ExistingElement_ReturnsTrue()
    {
        var list = new LinkedList<int>(new[] { 1, 2, 3, 4, 5 });
        
        Assert.True(list.Contains(3));
    }

    [Fact]
    public void Contains_NonExistingElement_ReturnsFalse()
    {
        var list = new LinkedList<int>(new[] { 1, 2, 3, 4, 5 });
        
        Assert.False(list.Contains(100));
    }
}

public class QueueTests
{
    [Fact]
    public void Enqueue_AddsElementToEnd()
    {
        var queue = new Queue<int>();
        
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        
        Assert.Equal(3, queue.Count);
        Assert.Equal(1, queue.Peek()); // Первый добавленный
    }

    [Fact]
    public void Dequeue_RemovesFirstElement()
    {
        var queue = new Queue<int>(new[] { 1, 2, 3 });
        
        int dequeued = queue.Dequeue();
        
        Assert.Equal(1, dequeued);
        Assert.Equal(2, queue.Count);
        Assert.Equal(2, queue.Peek());
    }

    [Fact]
    public void Dequeue_FIFO_Order()
    {
        var queue = new Queue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        
        Assert.Equal(1, queue.Dequeue());
        Assert.Equal(2, queue.Dequeue());
        Assert.Equal(3, queue.Dequeue());
    }

    [Fact]
    public void Contains_ExistingElement_ReturnsTrue()
    {
        var queue = new Queue<int>(new[] { 1, 2, 3, 4, 5 });
        
        Assert.True(queue.Contains(3));
    }

    [Fact]
    public void Contains_NonExistingElement_ReturnsFalse()
    {
        var queue = new Queue<int>(new[] { 1, 2, 3, 4, 5 });
        
        Assert.False(queue.Contains(100));
    }
}

public class StackTests
{
    [Fact]
    public void Push_AddsElementToTop()
    {
        var stack = new Stack<int>();
        
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        
        Assert.Equal(3, stack.Count);
        Assert.Equal(3, stack.Peek()); // Последний добавленный
    }

    [Fact]
    public void Pop_RemovesTopElement_Corrected()
    {
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3); // 3 на вершине
        
        int popped = stack.Pop();
        
        Assert.Equal(3, popped); // Последний добавленный
        Assert.Equal(2, stack.Count);
        Assert.Equal(2, stack.Peek()); // Теперь 2 на вершине
    }
    
    [Fact]
    public void Push_Pop_LIFO_Order()
    {
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        
        Assert.Equal(3, stack.Pop()); // Последний добавленный — первый извлечённый
        Assert.Equal(2, stack.Pop());
        Assert.Equal(1, stack.Pop());
    }

    [Fact]
    public void Contains_ExistingElement_ReturnsTrue()
    {
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        
        Assert.True(stack.Contains(2));
    }

    [Fact]
    public void Contains_NonExistingElement_ReturnsFalse()
    {
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        
        Assert.False(stack.Contains(100));
    }
}

public class ImmutableListTests
{
    [Fact]
    public void Add_ReturnsNewList_OriginalUnchanged()
    {
        var original = ImmutableList<int>.Empty.Add(1).Add(2);
        
        var modified = original.Add(3);
        
        Assert.Equal(2, original.Count); // Оригинал не изменился!
        Assert.Equal(3, modified.Count);
    }

    [Fact]
    public void Insert_AtBeginning_ReturnsNewList()
    {
        var original = ImmutableList<int>.Empty.Add(2).Add(3);
        
        var modified = original.Insert(0, 1);
        
        Assert.Equal(2, original.Count);
        Assert.Equal(3, modified.Count);
        Assert.Equal(1, modified[0]);
    }

    [Fact]
    public void RemoveAt_ReturnsNewList_OriginalUnchanged()
    {
        var original = ImmutableList<int>.Empty.Add(1).Add(2).Add(3);
        
        var modified = original.RemoveAt(0);
        
        Assert.Equal(3, original.Count);
        Assert.Equal(2, modified.Count);
        Assert.Equal(2, modified[0]);
    }

    [Fact]
    public void Contains_ExistingElement_ReturnsTrue()
    {
        var list = ImmutableList<int>.Empty.AddRange(new[] { 1, 2, 3, 4, 5 });
        
        Assert.True(list.Contains(3));
    }

    [Fact]
    public void Contains_NonExistingElement_ReturnsFalse()
    {
        var list = ImmutableList<int>.Empty.AddRange(new[] { 1, 2, 3, 4, 5 });
        
        Assert.False(list.Contains(100));
    }

    [Fact]
    public void IndexAccess_ReturnsCorrectElement()
    {
        var list = ImmutableList<int>.Empty.AddRange(new[] { 10, 20, 30, 40, 50 });
        
        Assert.Equal(10, list[0]);
        Assert.Equal(30, list[2]);
        Assert.Equal(50, list[4]);
    }

    [Fact]
    public void Immutability_ChainedOperations()
    {
        var list1 = ImmutableList<int>.Empty;
        var list2 = list1.Add(1);
        var list3 = list2.Add(2);
        var list4 = list3.Add(3);
        
        // Все версии сохраняются
        Assert.Equal(0, list1.Count);
        Assert.Equal(1, list2.Count);
        Assert.Equal(2, list3.Count);
        Assert.Equal(3, list4.Count);
    }
}
