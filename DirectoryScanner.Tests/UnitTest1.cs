using System.Reflection;
using System.Reflection.PortableExecutable;
using DirectoryScanner.Core;
using DirectoryScanner.Core.Classes;
namespace DirectoryScanner.Tests;

public class UnitTest1
{
    [Fact]
    public void TestOnlyDirectories()
    {
        CancellationTokenSource TokenSource = new();
        CancellationToken cancellationToken = TokenSource.Token;
        Scanner scanner = new(@"D:\User Files\testFolder\dir1", 5, cancellationToken);
        scanner.StartScanning();
        Assert.Equal(scanner.Root.Childs.Count, 2);
        Assert.True(scanner.Root.Childs.All(x => x is DirectoryEntity));
    }

    [Fact]
    public void TestOnlyFiles()
    {
        CancellationTokenSource TokenSource = new();
        CancellationToken cancellationToken = TokenSource.Token;
        Scanner scanner = new(@"D:\User Files\testFolder\dir2", 5, cancellationToken);
        scanner.StartScanning();
        Assert.Equal(scanner.Root.Childs.Count, 2);
        Assert.True(scanner.Root.Childs.All(x => x is RegularFileEntity));
    }

    [Fact]
    public void TestFileName()
    {
        CancellationTokenSource TokenSource = new();
        CancellationToken cancellationToken = TokenSource.Token;
        Scanner scanner = new(@"D:\User Files\testFolder\dir3", 5, cancellationToken);
        scanner.StartScanning();
        Assert.Equal(scanner.Root.Childs.Count, 1);
        Assert.Equal(scanner.Root.Childs[0].FullPath, @"D:\User Files\testFolder\dir3\wallhaven-8oll8y.jpg");
        Assert.True(scanner.Root.Childs[0] is RegularFileEntity);
    }

    [Fact]
    public void TestDirectoryName()
    {
        CancellationTokenSource TokenSource = new();
        CancellationToken cancellationToken = TokenSource.Token;
        Scanner scanner = new(@"D:\User Files\testFolder\dir4", 5, cancellationToken);
        scanner.StartScanning();
        Assert.Equal(scanner.Root.Childs.Count, 1);
        Assert.Equal(scanner.Root.Childs[0].FullPath, @"D:\User Files\testFolder\dir4\InnerDir");
        Assert.True(scanner.Root.Childs[0] is DirectoryEntity);
    }
    [Fact]
    public void TestEmptyDirectory()
    {
        CancellationTokenSource TokenSource = new();
        CancellationToken cancellationToken = TokenSource.Token;
        Scanner scanner = new(@"D:\User Files\testFolder\dir5", 5, cancellationToken); 
        scanner.StartScanning();
        Assert.Equal(scanner.Root.Childs.Count, 0);
    }
}