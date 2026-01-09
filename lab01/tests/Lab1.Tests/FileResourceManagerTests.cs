using Xunit;
using Lab1.Services;

namespace Lab1.Tests;

public class FileResourceManagerTests : IDisposable
{
    private readonly string _testDirectory;

    public FileResourceManagerTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "FRMTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, true);
    }

    private string GetTestFilePath(string fileName) => Path.Combine(_testDirectory, fileName);

    [Fact]
    public void OpenForWriting_CreatesFile()
    {
        string path = GetTestFilePath("write_test.txt");
        using (var manager = new FileResourceManager(path))
        {
            manager.OpenForWriting(path);
        }
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void WriteLine_WritesTextToFile()
    {
        string path = GetTestFilePath("write_line.txt");
        using (var manager = new FileResourceManager(path))
        {
            manager.OpenForWriting(path);
            manager.WriteLine("Hello, World!");
        }
        string content = File.ReadAllText(path);
        Assert.Contains("Hello, World!", content);
    }

    [Fact]
    public void OpenForReading_FileNotExists_ThrowsException()
    {
        string path = GetTestFilePath("nonexistent.txt");
        using var manager = new FileResourceManager(path);
        Assert.Throws<FileNotFoundException>(() => manager.OpenForReading());
    }

    [Fact]
    public void ReadAllText_ReturnsFileContent()
    {
        string path = GetTestFilePath("read_test.txt");
        File.WriteAllText(path, "Test content");

        using var manager = new FileResourceManager(path);
        manager.OpenForReading();
        string content = manager.ReadAllText();

        Assert.Equal("Test content", content);
    }

    [Fact]
    public void Dispose_PreventsSubsequentOperations()
    {
        string path = GetTestFilePath("dispose_test.txt");
        var manager = new FileResourceManager(path);
        manager.OpenForWriting(path);
        manager.Dispose();

        Assert.Throws<ObjectDisposedException>(() => manager.WriteLine("Test"));
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        string path = GetTestFilePath("multi_dispose.txt");
        var manager = new FileResourceManager(path);
        manager.OpenForWriting(path);

        manager.Dispose();
        manager.Dispose();
        manager.Dispose();
    }
}
