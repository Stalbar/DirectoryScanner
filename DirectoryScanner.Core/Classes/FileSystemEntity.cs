using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DirectoryScanner.Core.Classes;

public abstract class FileSystemEntity: INotifyPropertyChanged
{
    public abstract long Size { get; }

    public string FullPath { get; }

    private DirectoryEntity? _parentDirectory;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public string PercentSize { get => _parentDirectory == null ? "100%" : ((this.Size * 1.0 / _parentDirectory.Size) * 100).ToString("0.0000") + "%"; }

    public FileSystemEntity(string fullPath, DirectoryEntity? parentDirectory)
    {
        FullPath = fullPath;
        _parentDirectory = parentDirectory;
    }

}