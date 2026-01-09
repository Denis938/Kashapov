using Xunit;
using Lab3;

namespace Lab3.Tests;

public class SimpleListTests
{
    [Fact]
    public void Add_SingleItem_CountIsOne()
    {
        var list = new SimpleList<int>();
        
        list.Add(42);
        
        Assert.Equal(1, list.Count);
        Assert.Equal(42, list[0]);
    }

    [Fact]
    public void Add_MultipleItems_AllItemsPresent()
    {
        var list = new SimpleList<int>();
        
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void Add_ExceedsInitialCapacity_ResizesCorrectly()
    {
        var list = new SimpleList<int>(2);
        
        for (int i = 1; i <= 5; i++)
            list.Add(i);
        
        Assert.Equal(5, list.Count);
        Assert.Equal(5, list[4]);
    }

    [Fact]
    public void IndexOf_ExistingItem_ReturnsCorrectIndex()
    {
        var list = new SimpleList<string>();
        list.Add("a");
        list.Add("b");
        list.Add("c");
        
        Assert.Equal(1, list.IndexOf("b"));
    }

    [Fact]
    public void IndexOf_NonExistingItem_ReturnsMinusOne()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        list.Add(2);
        
        Assert.Equal(-1, list.IndexOf(999));
    }

    [Fact]
    public void Remove_ExistingItem_RemovesAndShifts()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        bool removed = list.Remove(2);
        
        Assert.True(removed);
        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void Remove_NonExistingItem_ReturnsFalse()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        
        Assert.False(list.Remove(999));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Insert_AtMiddle_ShiftsElements()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        list.Add(3);
        
        list.Insert(1, 2);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(2, list[1]);
    }

    [Fact]
    public void RemoveAt_ValidIndex_RemovesElement()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        list.RemoveAt(1);
        
        Assert.Equal(2, list.Count);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void RemoveAt_InvalidIndex_ThrowsException()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(5));
    }

    [Fact]
    public void Clear_NonEmptyList_BecomesEmpty()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        list.Add(2);
        
        list.Clear();
        
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        var list = new SimpleList<string>();
        list.Add("hello");
        
        Assert.True(list.Contains("hello"));
        Assert.False(list.Contains("world"));
    }

    [Fact]
    public void Foreach_IteratesAllElements()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        int sum = 0;
        foreach (var item in list)
            sum += item;
        
        Assert.Equal(6, sum);
    }

    [Fact]
    public void Indexer_InvalidIndex_ThrowsException()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[10]);
    }

    [Fact]
    public void CopyTo_ValidArray_CopiesElements()
    {
        var list = new SimpleList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        var array = new int[5];
        
        list.CopyTo(array, 1);
        
        Assert.Equal(new[] { 0, 1, 2, 3, 0 }, array);
    }
}
