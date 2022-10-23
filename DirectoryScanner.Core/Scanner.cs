using System.Reflection.PortableExecutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
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
        var subEntities = new List<FileSystemEntity>();

        var subFiles = GetDirectorySubFiles(directoryToProcess);
        ProcessAllSubFiles(subFiles, directoryToProcess, subEntities);

        var subDirectories = GetDirectorySubDirectories(directoryToProcess);
        ProcessAllSubDirectories(subDirectories, directoryToProcess, subEntities);

        directoryToProcess.Childs.AddRange(subEntities);
        _threads.TryRemove(new(Thread.CurrentThread, Environment.CurrentManagedThreadId));
    }

    private string[] GetDirectorySubFiles(DirectoryEntity directory)
    {
        string[] files = Directory.GetFiles(directory.FullPath);
        return files;
    }

    private void ProcessAllSubFiles(string[] subFiles, DirectoryEntity parentDirectory, List<FileSystemEntity> subEntities)
    {
        foreach (var subFile in subFiles)
            ProcessSingleSubFile(subFile, parentDirectory, subEntities);
    }

    private void ProcessSingleSubFile(string subFile, DirectoryEntity parentDirectory, List<FileSystemEntity> subEntities)
    {
        var fileSize = new FileInfo(subFile).Length;
        var fileEntity = new RegularFileEntity(subFile, parentDirectory, fileSize);
        subEntities.Add(fileEntity);
    }

    private string[] GetDirectorySubDirectories(DirectoryEntity directory)
    {
        string[] directories = Directory.GetDirectories(directory.FullPath);
        return directories;
    }

    private void ProcessAllSubDirectories(string[] subDirectories, DirectoryEntity parentDirectory, List<FileSystemEntity> subEntities)
    {
        foreach (var subDirectory in subDirectories)
            ProcessSingleDirectory(subDirectory, parentDirectory, subEntities);
    }

    private void ProcessSingleDirectory(string subDirectory, DirectoryEntity parentDirectory, List<FileSystemEntity> subEntities)
    {
        var directoryEntity = new DirectoryEntity(subDirectory, parentDirectory);
        _directoriesToScan.Enqueue(directoryEntity);
        subEntities.Add(directoryEntity);
    }

    public void Output()
    {
        Output("", _rootDirectory);
    }

    private void Output(string indent, DirectoryEntity root)
    {
        Console.WriteLine($"{indent}{root.FullPath} {root.Size} {root.PercentSize}");
        foreach (var child in root.Childs)
        {
            if (child is RegularFileEntity)
            {
                Console.WriteLine($"{indent}\t{child.FullPath} {child.Size} {child.PercentSize}");
            }
            else if (child is DirectoryEntity)
            {
                Output(indent + "\t", (DirectoryEntity)child);
            }
        }
    }
}