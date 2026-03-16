namespace Philiprehberger.TempFile;

/// <summary>
/// A disposable wrapper around a temporary directory that automatically
/// deletes the directory and all its contents when disposed.
/// </summary>
public sealed class TempDirectory : IDisposable, IAsyncDisposable
{
    private bool _disposed;

    private TempDirectory(string path)
    {
        Path = path;
    }

    /// <summary>
    /// Gets the full path of the temporary directory.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the directory name (last segment of the path).
    /// </summary>
    public string DirectoryName => System.IO.Path.GetFileName(Path);

    /// <summary>
    /// Creates a new temporary directory with a random name in the system temp directory.
    /// </summary>
    /// <returns>A new <see cref="TempDirectory"/> instance.</returns>
    public static TempDirectory Create()
    {
        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
        Directory.CreateDirectory(path);
        return new TempDirectory(path);
    }

    /// <summary>
    /// Creates a new temporary directory with the specified prefix in the system temp directory.
    /// </summary>
    /// <param name="prefix">A prefix to prepend to the random directory name.</param>
    /// <returns>A new <see cref="TempDirectory"/> instance.</returns>
    public static TempDirectory Create(string prefix)
    {
        var path = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            prefix + System.IO.Path.GetRandomFileName());

        Directory.CreateDirectory(path);
        return new TempDirectory(path);
    }

    /// <summary>
    /// Recursively deletes the temporary directory and all its contents.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
        catch (IOException)
        {
            // Best-effort cleanup — directory or contents may be locked.
        }
    }

    /// <summary>
    /// Asynchronously deletes the temporary directory and all its contents.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
