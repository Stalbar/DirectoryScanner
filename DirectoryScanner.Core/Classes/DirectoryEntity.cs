using System.IO.IsolatedStorage;
namespace DirectoryScanner.Core.Classes;

public class DirectoryEntity: FileSystemEntity
{
    private List<FileSystemEntity> _childs;
    private bool _isFullProcessed = false;

    public List<FileSystemEntity> Childs 
    {
        get => _childs;
        set
        {
            _childs = value;
            OnPropertyChanged(nameof(Childs));
        }
    }
    public bool IsFullProcessed
    {
        get => _isFullProcessed;
        set
        {
            _isFullProcessed = value;
            OnPropertyChanged(nameof(Size));
            OnPropertyChanged(nameof(Childs));
            OnPropertyChanged(nameof(PercentSize));
        }
    }

    public DirectoryEntity(string fullPath, DirectoryEntity? parentDirectory) 
        : base(fullPath, parentDirectory)
    {
        _childs = new();
    }

    public override long? Size => IsFullProcessed && Childs.All(x => x.Size.HasValue) ? Childs.Sum(x => x.Size) : null;
}