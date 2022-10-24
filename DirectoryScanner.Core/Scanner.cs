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

    private CancellationToken _cancellationToken;

    public DirectoryEntity Root { get => _rootDirectory; }

    public Scanner(string fullPath, int maxThreadCount, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _threads = new();
        _maxThreadCount = maxThreadCount;
        _semaphore = new(_maxThreadCount, _maxThreadCount);
        _directoriesToScan = new();
        _rootDirectory = new DirectoryEntity(fullPath, null);
        _directoriesToScan.Enqueue(_rootDirectory);
    }

    public void StartScanning()
    {
        while (!_directoriesToScan.IsEmpty || !_threads.IsEmpty && !_cancellationToken.IsCancellationRequested)
        {
            _semaphore.WaitOne();
            if (_directoriesToScan.TryDequeue(out DirectoryEntity directoryToProcess) && !_cancellationToken.IsCancellationRequested)
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

        directoryToProcess.Childs = subEntities;
        directoryToProcess.IsFullProcessed = !_cancellationToken.IsCancellationRequested;
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
            if (!_cancellationToken.IsCancellationRequested)
                ProcessSingleSubFile(subFile, parentDirectory, subEntities);
            else
                return;
    }

    private void ProcessSingleSubFile(string subFile, DirectoryEntity parentDirectory, List<FileSystemEntity> subEntities)
    {
        if (_cancellationToken.IsCancellationRequested)
            return;
        var fileSize = new FileInfo(subFile).Length;
        var fileEntity = new RegularFileEntity(subFile, parentDirectory, fileSize);
        subEntities.Add(fileEntity);
        Thread.Sleep(500);
    }

    private string[] GetDirectorySubDirectories(DirectoryEntity directory)
    {
        string[] directories = Directory.GetDirectories(directory.FullPath);
        return directories;
    }

    private void ProcessAllSubDirectories(string[] subDirectories, DirectoryEntity parentDirectory, List<FileSystemEntity> subEntities)
    {
        foreach (var subDirectory in subDirectories)
            if (!_cancellationToken.IsCancellationRequested)
                ProcessSingleDirectory(subDirectory, parentDirectory, subEntities);
            else
                return;
    }

    private void ProcessSingleDirectory(string subDirectory, DirectoryEntity parentDirectory, List<FileSystemEntity> subEntities)
    {
        if (_cancellationToken.IsCancellationRequested)
            return;
        var directoryEntity = new DirectoryEntity(subDirectory, parentDirectory);
        _directoriesToScan.Enqueue(directoryEntity);
        subEntities.Add(directoryEntity);
        Thread.Sleep(500);
    }
}