using System.Text;

namespace Lab1.Services;

public class FileResourceManager : IDisposable
{
    private FileStream _fileStream;
    private StreamWriter _writer;
    private StreamReader _reader;
    private bool _disposed;
    private readonly string _filePath;

    public FileResourceManager(string filepath)
    {
        _filePath = filepath;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FileResourceManager));
    }

    public void OpenForWriting(string path)
    {
        ThrowIfDisposed();
        _fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write);
        _writer = new StreamWriter(_fileStream, Encoding.UTF8);
    }

    public void OpenForReading()
    {
        ThrowIfDisposed();
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("Файл не найден", _filePath);
        _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
        _reader = new StreamReader(_fileStream, Encoding.UTF8);
    }

    public void WriteLine(string text)
    {
        ThrowIfDisposed();
        _writer?.WriteLine(text);
    }

    public string ReadAllText()
    {
        ThrowIfDisposed();
        return _reader?.ReadToEnd() ?? string.Empty;
    }

    public void AppendText(string text)
    {
        ThrowIfDisposed();
        using var fs = new FileStream(_filePath, FileMode.Append, FileAccess.Write);
        using var writer = new StreamWriter(fs, Encoding.UTF8);
        writer.Write(text);
    }

    public FileInfo GetFileInfo()
    {
        ThrowIfDisposed();
        var fileInfo = new FileInfo(_filePath);
        if (!fileInfo.Exists)
            throw new FileNotFoundException("File not found", _filePath);
        return fileInfo;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _writer?.Dispose();
                _reader?.Dispose();
                _fileStream?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~FileResourceManager()
    {
        Dispose(false);
    }
}