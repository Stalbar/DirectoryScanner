using System.Collections.ObjectModel;
using System.Windows;
using Ookii.Dialogs.Wpf;
using DirectoryScanner.Core.Classes;
using DirectoryScanner.Core;
using System.Threading;
using System.Windows.Controls;

namespace DirectoryScanner.WPF
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<DirectoryEntity> nodes = new();
        public CancellationTokenSource cancellationTokenSource;
        public MainWindow()
        {

            InitializeComponent();
        }

        private void InputFile_Click(object sender, RoutedEventArgs e)
        {

            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();

            if (dlg.ShowDialog() == true)
            {
                cancellationTokenSource = new();
                CancellationToken token = cancellationTokenSource.Token;
                treeView.ItemsSource = nodes;
                nodes.Clear();
                Scanner scanner = new Scanner(dlg.SelectedPath, 4, token);
                nodes.Add(scanner.Root);
                Thread thread = new(scanner.StartScanning);
                thread.Start();
            }
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
