# Philiprehberger.TempFile

[![CI](https://github.com/philiprehberger/dotnet-temp-file/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-temp-file/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.TempFile.svg)](https://www.nuget.org/packages/Philiprehberger.TempFile)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-temp-file)](LICENSE)

Safe temporary file and directory management with automatic cleanup via IDisposable and IAsyncDisposable.

## Installation

```bash
dotnet add package Philiprehberger.TempFile
```

## Usage

### Temporary files

```csharp
using Philiprehberger.TempFile;

// Create a temp file (deleted on dispose)
using var file = TempFile.Create();
Console.WriteLine(file.Path);

// Create with a specific extension
using var csvFile = TempFile.Create(".csv");

// Create with string content
using var log = TempFile.WithContent("Hello, world!");

// Create with stream content
using var copy = TempFile.WithContent(someStream);

// Create in a specific directory
using var scoped = TempFile.InDirectory("/tmp/my-app");

// Access the underlying FileStream
file.Stream.Write(data);
```

### Temporary directories

```csharp
using Philiprehberger.TempFile;

// Create a temp directory (recursively deleted on dispose)
using var dir = TempDirectory.Create();
Console.WriteLine(dir.Path);

// Create with a prefix
using var prefixed = TempDirectory.Create("myapp-");
```

### Async disposal

```csharp
await using var file = TempFile.Create();
await using var dir = TempDirectory.Create();
```

## API

### `TempFile`

| Member | Description |
|--------|-------------|
| `Create()` | Creates a temp file with a random name |
| `Create(extension)` | Creates a temp file with the specified extension |
| `WithContent(string)` | Creates a temp file with string content |
| `WithContent(Stream)` | Creates a temp file with content from a stream |
| `InDirectory(dir)` | Creates a temp file in the specified directory |
| `Path` | Full path of the temporary file |
| `FileName` | File name without directory |
| `Stream` | Lazily-opened `FileStream` for reading and writing |

### `TempDirectory`

| Member | Description |
|--------|-------------|
| `Create()` | Creates a temp directory with a random name |
| `Create(prefix)` | Creates a temp directory with a prefix |
| `Path` | Full path of the temporary directory |
| `DirectoryName` | Directory name (last path segment) |

### `TempFileOptions`

| Property | Description |
|----------|-------------|
| `Extension` | File extension (e.g., ".txt") |
| `Directory` | Target directory for the temp file |
| `Prefix` | Prefix for the file name |

## Development

```bash
dotnet build src/Philiprehberger.TempFile.csproj --configuration Release
```

## License

MIT
