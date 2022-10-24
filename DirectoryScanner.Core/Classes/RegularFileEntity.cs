namespace DirectoryScanner.Core.Classes;

public class RegularFileEntity : FileSystemEntity
{
    private long? _size;
    public override long? Size => _size;

    public RegularFileEntity(string fullPath, DirectoryEntity? parentDirectory, long? size)
        : base(fullPath, parentDirectory)
    {
        _size = size;
    }
}