namespace DirectoryScanner.Core.Classes;

public class DirectoryEntity: FileSystemEntity
{
    private List<FileSystemEntity> _childs;
    
    public List<FileSystemEntity> Childs 
    {
        get => _childs;
        set
        {
            _childs = value;
            OnPropertyChanged(nameof(Childs));
        }
    }

    public DirectoryEntity(string fullPath, DirectoryEntity? parentDirectory) 
        : base(fullPath, parentDirectory)
    {
        _childs = new();
    }

    public override long Size => Childs.Sum(x => x.Size);
}