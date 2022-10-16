namespace DirectoryScanner.Core.Classes;

public class DirectoryEntity: FileSystemEntity
{

    public List<FileSystemEntity> Childs { get; set; }

    public DirectoryEntity(string fullPath, DirectoryEntity? parentDirectory) 
        : base(fullPath, parentDirectory)
    {
        Childs = new();
    }

    public override long Size => Childs.Sum(x => x.Size);
}