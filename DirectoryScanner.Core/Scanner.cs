using System.Collections.Concurrent;
using DirectoryScanner.Core.Classes;

namespace DirectoryScanner.Core;

public class Scanner
{
    private DirectoryEntity _rootDirectory;
    private ConcurrentQueue<DirectoryEntity> _directoriesToScan;

    public Scanner(string fullPath)
    {
        _directoriesToScan = new();
        _rootDirectory = new DirectoryEntity(fullPath, null);
        _directoriesToScan.Enqueue(_rootDirectory);
    }

    public void StartScanning()
    {
        while (!_directoriesToScan.IsEmpty)
        {
            if (_directoriesToScan.TryDequeue(out DirectoryEntity? directoryToProcess))
            {
                ProcessFileSystemEntity(directoryToProcess);
            }
        }
    }

    private void ProcessFileSystemEntity(DirectoryEntity directoryToProcess)
    {
        var subDirectories = Directory.GetDirectories(directoryToProcess.FullPath);
        var subEntities = new List<FileSystemEntity>();
        foreach (var subDirectory in subDirectories)
        {
            var directoryEntity = new DirectoryEntity(subDirectory, directoryToProcess);
            _directoriesToScan.Enqueue(directoryEntity);
            subEntities.Add(directoryEntity);
        }
        var subFiles = Directory.GetFiles(directoryToProcess.FullPath);
        foreach (var subFile in subFiles)
        {
            var fileSize = new FileInfo(subFile).Length;
            var fileEntity = new RegularFileEntity(subFile, directoryToProcess, fileSize);
            subEntities.Add(fileEntity);
        }

        directoryToProcess.Childs.AddRange(subEntities);
    }
}