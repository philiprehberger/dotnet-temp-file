namespace Philiprehberger.TempFile;

/// <summary>
/// A disposable wrapper around a temporary file that automatically deletes
/// the file when disposed.
/// </summary>
public sealed class TempFile : IDisposable, IAsyncDisposable
{
    private FileStream? _stream;
    private bool _disposed;

    private TempFile(string path)
    {
        Path = path;
    }

    /// <summary>
    /// Gets the full path of the temporary file.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the file name (without directory) of the temporary file.
    /// </summary>
    public string FileName => System.IO.Path.GetFileName(Path);

    /// <summary>
    /// Gets a <see cref="FileStream"/> for reading and writing the temporary file.
    /// The stream is lazily opened on first access.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the instance has been disposed.</exception>
    public FileStream Stream
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _stream ??= new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
        }
    }

    /// <summary>
    /// Creates a new temporary file with a random name in the system temp directory.
    /// </summary>
    /// <returns>A new <see cref="TempFile"/> instance.</returns>
    public static TempFile Create()
    {
        var path = System.IO.Path.GetTempFileName();
        return new TempFile(path);
    }

    /// <summary>
    /// Creates a new temporary file with the specified extension in the system temp directory.
    /// </summary>
    /// <param name="extension">The file extension, with or without a leading dot (e.g., ".txt" or "txt").</param>
    /// <returns>A new <see cref="TempFile"/> instance.</returns>
    public static TempFile Create(string extension)
    {
        if (!extension.StartsWith('.'))
            extension = "." + extension;

        var path = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            System.IO.Path.GetRandomFileName() + extension);

        File.Create(path).Dispose();
        return new TempFile(path);
    }

    /// <summary>
    /// Creates a new temporary file with the specified string content.
    /// </summary>
    /// <param name="content">The text content to write to the file.</param>
    /// <returns>A new <see cref="TempFile"/> instance.</returns>
    public static TempFile WithContent(string content)
    {
        var tempFile = Create();
        File.WriteAllText(tempFile.Path, content);
        return tempFile;
    }

    /// <summary>
    /// Creates a new temporary file with content copied from the specified stream.
    /// </summary>
    /// <param name="content">The stream whose content will be copied to the temporary file.</param>
    /// <returns>A new <see cref="TempFile"/> instance.</returns>
    public static TempFile WithContent(Stream content)
    {
        var tempFile = Create();
        using var fileStream = File.OpenWrite(tempFile.Path);
        content.CopyTo(fileStream);
        return tempFile;
    }

    /// <summary>
    /// Creates a new temporary file in the specified directory.
    /// </summary>
    /// <param name="directory">The directory in which to create the temporary file.</param>
    /// <returns>A new <see cref="TempFile"/> instance.</returns>
    public static TempFile InDirectory(string directory)
    {
        Directory.CreateDirectory(directory);
        var path = System.IO.Path.Combine(directory, System.IO.Path.GetRandomFileName());
        File.Create(path).Dispose();
        return new TempFile(path);
    }

    /// <summary>
    /// Deletes the temporary file and releases all resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _stream?.Dispose();
        _stream = null;

        try
        {
            if (File.Exists(Path))
                File.Delete(Path);
        }
        catch (IOException)
        {
            // Best-effort cleanup — file may be locked by another process.
        }
    }

    /// <summary>
    /// Asynchronously deletes the temporary file and releases all resources.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_stream is not null)
        {
            await _stream.DisposeAsync();
            _stream = null;
        }

        try
        {
            if (File.Exists(Path))
                File.Delete(Path);
        }
        catch (IOException)
        {
            // Best-effort cleanup — file may be locked by another process.
        }
    }
}
