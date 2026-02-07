using Avalonia.Interactivity;
using FluentAvalonia.UI.Windowing;
namespace RocketLauncherRemake
{
    public partial class MainWindow : AppWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.TitleBar.Height = 48;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}