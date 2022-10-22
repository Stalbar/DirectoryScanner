namespace DirectoryScanner.Core.Classes;

public abstract class FileSystemEntity
{
    public abstract long Size { get; }

    public string FullPath { get; }

    private DirectoryEntity? _parentDirectory;

    public string PercentSize { get => _parentDirectory == null ? "100%" : ((this.Size * 1.0 / _parentDirectory.Size) * 100).ToString("0.0000") + "%"; }

    public FileSystemEntity(string fullPath, DirectoryEntity? parentDirectory)
    {
        FullPath = fullPath;
        _parentDirectory = parentDirectory;
    }

}