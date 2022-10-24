using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DirectoryScanner.Core.Classes;

public abstract class FileSystemEntity : INotifyPropertyChanged
{
    public abstract long? Size { get; }

    public string FullPath { get; }

    private DirectoryEntity? _parentDirectory;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    private double? PercentValue => _parentDirectory != null && _parentDirectory.Size.HasValue ? this.Size / (_parentDirectory.Size * 1.0) : null;

    public string PercentSize => PercentValue.HasValue ? $"{PercentValue * 100:0.00}%" : "-";
    public FileSystemEntity(string fullPath, DirectoryEntity? parentDirectory)
    {
        FullPath = fullPath;
        _parentDirectory = parentDirectory;
        if (_parentDirectory != null)
            PropertyChanged += (x, y) => _parentDirectory.OnPropertyChanged(y.PropertyName);
    }

}