namespace DirectoryScanner.Core.Classes;

public abstract class FileSystemEntity
{
    public abstract long Size { get; }

    private string FullPath;

    private DirectoryEntity? _parentDirectory;

    public FileSystemEntity(string fullPath, DirectoryEntity parentDirectory)
    {
        FullPath = fullPath;
        _parentDirectory = parentDirectory;
    }
}