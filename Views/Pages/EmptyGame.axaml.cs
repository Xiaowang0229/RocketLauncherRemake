using Avalonia.Controls;
using Avalonia.Interactivity;
using RocketLauncherRemake.Utils;

namespace RocketLauncherRemake;

public partial class EmptyGame : UserControl
{
    public EmptyGame()
    {
        InitializeComponent();
    }

    private void Import_Click(object sender,RoutedEventArgs e)
    {
        Variables._MainWindow.RootFrame.Navigate(typeof(ImportPage));
    }
}