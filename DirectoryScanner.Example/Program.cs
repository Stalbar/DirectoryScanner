using DirectoryScanner.Core;
namespace DirectoryScanner.Example;

public class Program
{
    static void Main(string[] args)
    {
        Scanner scanner = new Scanner(@"D:\User Files\testFolder");
        scanner.StartScanning();
    }
}