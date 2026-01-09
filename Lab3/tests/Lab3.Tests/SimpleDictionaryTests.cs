using Xunit;
using Lab3;

namespace Lab3.Tests;

public class SimpleDictionaryTests
{
    [Fact]
    public void Add_SinglePair_CountIsOne()
    {
        var dict = new SimpleDictionary<string, int>();
        
        dict.Add("one", 1);
        
        Assert.Equal(1, dict.Count);
        Assert.Equal(1, dict["one"]);
    }

    [Fact]
    public void Add_DuplicateKey_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key", 1);
        
        Assert.Throws<ArgumentException>(() => dict.Add("key", 2));
    }

    [Fact]
    public void Indexer_Set_UpdatesValue()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key", 1);
        
        dict["key"] = 100;
        
        Assert.Equal(100, dict["key"]);
    }

    [Fact]
    public void Indexer_Set_NewKey_AddsValue()
    {
        var dict = new SimpleDictionary<string, int>();
        
        dict["newkey"] = 42;
        
        Assert.Equal(1, dict.Count);
        Assert.Equal(42, dict["newkey"]);
    }

    [Fact]
    public void Indexer_Get_NonExistingKey_ThrowsException()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.Throws<KeyNotFoundException>(() => dict["missing"]);
    }

    [Fact]
    public void Remove_ExistingKey_ReturnsTrue()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key", 1);
        
        bool removed = dict.Remove("key");
        
        Assert.True(removed);
        Assert.Equal(0, dict.Count);
        Assert.False(dict.ContainsKey("key"));
    }

    [Fact]
    public void Remove_NonExistingKey_ReturnsFalse()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.False(dict.Remove("missing"));
    }

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var dict = new SimpleDictionary<int, string>();
        dict.Add(42, "answer");
        
        Assert.True(dict.ContainsKey(42));
        Assert.False(dict.ContainsKey(100));
    }

    [Fact]
    public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("test", 123);
        
        bool found = dict.TryGetValue("test", out int value);
        
        Assert.True(found);
        Assert.Equal(123, value);
    }

    [Fact]
    public void TryGetValue_NonExistingKey_ReturnsFalse()
    {
        var dict = new SimpleDictionary<string, int>();
        
        bool found = dict.TryGetValue("missing", out _);
        
        Assert.False(found);
    }

    [Fact]
    public void Clear_NonEmptyDict_BecomesEmpty()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("a", 1);
        dict.Add("b", 2);
        
        dict.Clear();
        
        Assert.Equal(0, dict.Count);
    }

    [Fact]
    public void Foreach_IteratesAllPairs()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("a", 1);
        dict.Add("b", 2);
        dict.Add("c", 3);
        
        int sum = 0;
        foreach (var kvp in dict)
            sum += kvp.Value;
        
        Assert.Equal(6, sum);
    }

    [Fact]
    public void Keys_ReturnsAllKeys()
    {
        var dict = new SimpleDictionary<int, string>();
        dict.Add(1, "a");
        dict.Add(2, "b");
        
        var keys = dict.Keys;
        
        Assert.Equal(2, keys.Count);
        Assert.Contains(1, keys);
        Assert.Contains(2, keys);
    }

    [Fact]
    public void Values_ReturnsAllValues()
    {
        var dict = new SimpleDictionary<int, string>();
        dict.Add(1, "a");
        dict.Add(2, "b");
        
        var values = dict.Values;
        
        Assert.Equal(2, values.Count);
        Assert.Contains("a", values);
        Assert.Contains("b", values);
    }

    [Fact]
    public void Resize_ManyItems_WorksCorrectly()
    {
        var dict = new SimpleDictionary<int, int>(4);
        
        for (int i = 0; i < 100; i++)
            dict.Add(i, i * 10);
        
        Assert.Equal(100, dict.Count);
        Assert.Equal(500, dict[50]);
    }
}
