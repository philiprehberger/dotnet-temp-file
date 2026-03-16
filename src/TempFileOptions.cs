namespace Philiprehberger.TempFile;

/// <summary>
/// Configuration options for creating temporary files.
/// </summary>
/// <param name="Extension">The file extension, with or without a leading dot (e.g., ".txt" or "txt"). Defaults to <c>null</c>.</param>
/// <param name="Directory">The directory in which to create the temporary file. Defaults to <c>null</c> (system temp directory).</param>
/// <param name="Prefix">A prefix to prepend to the random file name. Defaults to <c>null</c>.</param>
public record TempFileOptions(
    string? Extension = null,
    string? Directory = null,
    string? Prefix = null);
