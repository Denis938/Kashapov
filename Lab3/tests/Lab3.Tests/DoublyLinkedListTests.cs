using Xunit;
using Lab3;

namespace Lab3.Tests;

public class DoublyLinkedListTests
{
    [Fact]
    public void Add_Items_MaintainsOrder()
    {
        var list = new DoublyLinkedList<int>();
        
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void AddFirst_AddsAtBeginning()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(2);
        list.Add(3);
        
        list.AddFirst(1);
        
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
    }

    [Fact]
    public void Remove_MiddleElement_WorksCorrectly()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        list.Remove(2);
        
        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void Remove_FirstElement_WorksCorrectly()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        list.Remove(1);
        
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
    }

    [Fact]
    public void Remove_LastElement_WorksCorrectly()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        list.Remove(3);
        
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[1]);
    }

    [Fact]
    public void Indexer_AccessFromEnd_Works()
    {
        var list = new DoublyLinkedList<int>();
        for (int i = 0; i < 100; i++)
            list.Add(i);
        
        Assert.Equal(99, list[99]);
        Assert.Equal(95, list[95]);
    }

    [Fact]
    public void Insert_AtBeginning_WorksCorrectly()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(2);
        list.Add(3);
        
        list.Insert(0, 1);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
    }

    [Fact]
    public void Insert_AtMiddle_WorksCorrectly()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(3);
        
        list.Insert(1, 2);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(2, list[1]);
    }

    [Fact]
    public void Clear_NonEmptyList_BecomesEmpty()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        
        list.Clear();
        
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Foreach_IteratesAllElements()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        int sum = 0;
        foreach (var item in list)
            sum += item;
        
        Assert.Equal(6, sum);
    }

    [Fact]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        var list = new DoublyLinkedList<string>();
        list.Add("hello");
        
        Assert.True(list.Contains("hello"));
        Assert.False(list.Contains("world"));
    }

    [Fact]
    public void IndexOf_ExistingItem_ReturnsCorrectIndex()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(10);
        list.Add(20);
        list.Add(30);
        
        Assert.Equal(1, list.IndexOf(20));
        Assert.Equal(-1, list.IndexOf(999));
    }

    [Fact]
    public void RemoveAt_ValidIndex_RemovesElement()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        list.RemoveAt(1);
        
        Assert.Equal(2, list.Count);
        Assert.Equal(3, list[1]);
    }
}
