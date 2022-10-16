using System.Collections.Concurrent;
using DirectoryScanner.Core.Classes;

namespace DirectoryScanner.Core;

public class Scanner
{
    private ConcurrentDictionary<Thread, int> _threads;
    private int _maxThreadCount;
    private Semaphore _semaphore;
    private DirectoryEntity _rootDirectory;
    private ConcurrentQueue<DirectoryEntity> _directoriesToScan;

    public Scanner(string fullPath, int maxThreadCount)
    {
        _threads = new();
        _maxThreadCount = maxThreadCount;
        _semaphore = new(_maxThreadCount, _maxThreadCount);
        _directoriesToScan = new();
        _rootDirectory = new DirectoryEntity(fullPath, null);
        _directoriesToScan.Enqueue(_rootDirectory);
    }

    public void StartScanning()
    {
        while (!_directoriesToScan.IsEmpty || !_threads.IsEmpty)
        {
            _semaphore.WaitOne();
            if (_directoriesToScan.TryDequeue(out DirectoryEntity directoryToProcess))
            {
                Thread thread = new(obj => ProcessDirectory((DirectoryEntity)obj));
                _threads[thread] = thread.ManagedThreadId;
                thread.Start(directoryToProcess);
            }
            _semaphore.Release();
        }
    }

    private void ProcessDirectory(DirectoryEntity directoryToProcess)
    {
        _semaphore.WaitOne();

        var subEntities = new List<FileSystemEntity>();

        var subFiles = Directory.GetFiles(directoryToProcess.FullPath);
        foreach (var subFile in subFiles)
        {
            var fileSize = new FileInfo(subFile).Length;
            var fileEntity = new RegularFileEntity(subFile, directoryToProcess, fileSize);
            subEntities.Add(fileEntity);
        }

        var subDirectories = Directory.GetDirectories(directoryToProcess.FullPath);
        foreach (var subDirectory in subDirectories)
        {
            var directoryEntity = new DirectoryEntity(subDirectory, directoryToProcess);
            _directoriesToScan.Enqueue(directoryEntity);
            subEntities.Add(directoryEntity);
        }

        directoryToProcess.Childs.AddRange(subEntities);
        _threads.TryRemove(new (Thread.CurrentThread, Environment.CurrentManagedThreadId));
        _semaphore.Release();
    }

    public void Output()
    {
        Output("", _rootDirectory);
    }

    private void Output(string indent, DirectoryEntity root)
    {
        Console.WriteLine($"{indent}{root.FullPath} {root.Size}");
        foreach (var child in root.Childs)
        {
            if (child is RegularFileEntity)
            {
                Console.WriteLine($"{indent}\t{child.FullPath} {child.Size}");
            }
            else if (child is DirectoryEntity)
            {
                Output(indent + "\t", (DirectoryEntity)child);
            }
        }
    }
}