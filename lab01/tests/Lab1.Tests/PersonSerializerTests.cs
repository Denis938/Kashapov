using Xunit;
using Lab1.Models;
using Lab1.Services;

namespace Lab1.Tests;

public class PersonSerializerTests : IDisposable
{
    private readonly PersonSerializer _serializer = new();
    private readonly string _testDirectory;

    public PersonSerializerTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "Lab1Tests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, true);
    }

    private Person CreateSamplePerson()
    {
        return new Person("Ivan", "Ivanov", 25, "secret123", "p1", "+7-999-000-00-00", "ivan@example.com")
        {
            BirthDate = new DateTime(1999, 1, 1)
        };
    }

    private string GetTestFilePath(string fileName) => Path.Combine(_testDirectory, fileName);

    [Fact]
    public void SerializeToJson_ReturnsNonEmptyString()
    {
        var person = CreateSamplePerson();
        string json = _serializer.SerializeToJson(person);
        Assert.False(string.IsNullOrWhiteSpace(json));
    }

    [Fact]
    public void SerializeToJson_ExcludesPassword()
    {
        var person = CreateSamplePerson();
        string json = _serializer.SerializeToJson(person);
        Assert.DoesNotContain("secret123", json);
    }

    [Fact]
    public void SerializeToJson_HasIndentation()
    {
        var person = CreateSamplePerson();
        string json = _serializer.SerializeToJson(person);
        Assert.Contains("\n", json);
    }

    [Fact]
    public void DeserializeFromJson_RestoresObject()
    {
        var person = CreateSamplePerson();
        string json = _serializer.SerializeToJson(person);
        var restored = _serializer.DeserializeFromJson(json);

        Assert.Equal(person.FirstName, restored.FirstName);
        Assert.Equal(person.LastName, restored.LastName);
        Assert.Equal(person.Age, restored.Age);
    }

    [Fact]
    public void SaveToFile_CreatesFile()
    {
        var person = CreateSamplePerson();
        string path = GetTestFilePath("person.json");
        _serializer.SaveToFile(person, path);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void LoadFromFile_RestoresObject()
    {
        var person = CreateSamplePerson();
        string path = GetTestFilePath("person.json");
        _serializer.SaveToFile(person, path);
        var restored = _serializer.LoadFromFile(path);
        Assert.Equal(person.FirstName, restored.FirstName);
    }

    [Fact]
    public void LoadFromFile_FileNotExists_ThrowsException()
    {
        string path = GetTestFilePath("nonexistent.json");
        Assert.Throws<FileNotFoundException>(() => _serializer.LoadFromFile(path));
    }

    [Fact]
    public async Task SaveToFileAsync_CreatesFile()
    {
        var person = CreateSamplePerson();
        string path = GetTestFilePath("person_async.json");
        await _serializer.SaveToFileAsync(person, path);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void SaveLoadList_WorksCorrectly()
    {
        var people = new List<Person>
        {
            CreateSamplePerson(),
            new Person("Petr", "Petrov", 30, "pwd", "p2", "+7-111", "petr@test.com")
        };
        string path = GetTestFilePath("people.json");

        _serializer.SaveListToFile(people, path);
        var restored = _serializer.LoadListFromFile(path);

        Assert.Equal(2, restored.Count);
        Assert.Equal("Ivan", restored[0].FirstName);
        Assert.Equal("Petr", restored[1].FirstName);
    }
}
